function Queryable(array) {
    this._source = array;
    this._where = function (value) { return true; };
    this.where = function (predicate) { this._where = predicate; return this; };
    this.do = function (action) {
        var srcLength = this._source.length;
        for (var i = 0; i < srcLength; ++i) {
            if (this._where(this._source[i]))
                action(this._source[i]);
        }
    };
    this.any = function () {
        var srcLength = this._source.length;
        for (var i = 0; i < srcLength; ++i) {
            if (this._where(this._source[i]))
                return true;
        }
        return false;
    };
    this.toArray = function (select) {
        var result = [];
        this.do(function (o) { result.push(select(o)); });
        return result;
    };
}

Object.prototype.asQueryable = function () {
    return new Queryable(this);
};

function checkAll(checkBoxClass, checked) {
    document.getElementsByTagName('input')
        .asQueryable()
        .where(function (b) { return b.classList.contains(checkBoxClass); })
        .do(function (b) { b.checked = checked; });
}
function sortTable(tableId, columnIdx, numeric, toggle) {
    var direction = toggle.dataset.dir === 'true';
    toggle.dataset.dir = !direction;

    var sortMethod;
    var parseMethod;
    var checkMethod;
    if (numeric) {
        sortMethod = direction ? function (x, y) { return x[0] - y[0]; } : function (x, y) { return y[0] - x[0]; };
        parseMethod = function (x) { return parseFloat(x); };
        checkMethod = function (x) { return !isNaN(x); };
    } else {
        sortMethod = direction
            ? function (x, y) { return (x[0] > y[0]) ? 1 : ((x[0] < y[0]) ? -1 : 0); }
            : function (x, y) { return (x[0] < y[0]) ? 1 : ((x[0] > y[0]) ? -1 : 0); };
        parseMethod = function (x) { return x; };
        checkMethod = function (x) { return true; };
    }

    var tbl = document.getElementById(tableId).tBodies[0];
    var store = [];
    tbl.rows.asQueryable()
        .do(function (row) {
            var sortnr = parseMethod(row.cells[columnIdx].textContent || row.cells[columnIdx].innerText);
            if (checkMethod(sortnr)) store.push([sortnr, row]);
        });
    store.sort(sortMethod);
    store.asQueryable().do(function (row) { tbl.appendChild(row[1]); });
}

function applyFilter() {
    var getFilterValues = function (elementsName) {
        return document.getElementsByName(elementsName)
            .asQueryable()
            .where(function (e) { return e.checked; })
            .toArray(function (e) { return e.dataset.filterValue; });
    };

    var applyTo = function (tag, className, filter) {
        document.getElementsByTagName(tag)
            .asQueryable()
            .where(function (e) { return e.classList.contains(className); })
            .do(filter);
    };

    var statusFilterValues = getFilterValues('statusFilter');
    var categoryFilterValues = getFilterValues('categoryFilter');

    var statusFilter = function (element) {
        return statusFilterValues.asQueryable()
            .where(function (s) { return element.classList.contains(s); })
            .any();
    };

    var childFilter = function (feature) {
        return feature.getElementsByTagName('div')
            .asQueryable()
            .where(function (ch) { return ch.classList.contains('scenario') && ch.dataset.filtered == 'true'; })
            .any();
    };

    var categoryFilter = null;

    if (categoryFilterValues.length == 0 || categoryFilterValues[0] === 'all') {
        categoryFilter = function (scenario) { return true; };
        childFilter = statusFilter;
    } else if (categoryFilterValues[0] === 'without') {
        categoryFilter = function (scenario) { return scenario.dataset.categories == ''; };
    } else {
        categoryFilter = function (scenario) {
            return categoryFilterValues.asQueryable()
                .where(function (c) { return scenario.dataset.categories.indexOf(c) >= 0; })
                .any();
        };
    }

    applyTo('div', 'scenario', function (scenario) {
        scenario.dataset.filtered = statusFilter(scenario) && categoryFilter(scenario);
        scenario.className = scenario.className; //IE fix
    });
    applyTo('article', 'feature', function (feature) {
        feature.dataset.filtered = childFilter(feature);
        feature.className = feature.className; //IE fix
    });
}