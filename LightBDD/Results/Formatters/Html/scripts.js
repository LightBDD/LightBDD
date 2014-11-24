function checkAll(checkBoxClass, checked) {
    var buttons = document.getElementsByTagName('input');
    for (var i = buttons.length - 1; i >= 0; i--) {
        if (buttons[i].classList.contains(checkBoxClass))
            buttons[i].checked = checked;
    }
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
    var i;
    var len;

    for (i = 0, len = tbl.rows.length; i < len; i++) {
        var row = tbl.rows[i];
        var sortnr = parseMethod(row.cells[columnIdx].textContent || row.cells[columnIdx].innerText);
        if (checkMethod(sortnr)) store.push([sortnr, row]);
    }
    store.sort(sortMethod);
    for (i = 0, len = store.length; i < len; i++) {
        tbl.appendChild(store[i][1]);
    }
    store = null;
}

function filterCategory(categoryIdx) {
    var filter = function (tag, className) {
        var elements = document.getElementsByTagName(tag);
        for (var i = elements.length - 1; i >= 0; i--) {
            if (elements[i].classList.contains(className)) {
                elements[i].dataset.categoryFilter = elements[i].dataset.categories.indexOf(categoryIdx) >= 0;
            }
        }
    };

    filter('div', 'scenario');
    filter('article', 'feature');
}

function applyFilter() {
    var getFilterValues = function (elementsName) {
        var result = [];
        var elems = document.getElementsByName(elementsName);
        for (var i = elems.length - 1; i >= 0; i--) {
            if (elems[i].checked) {
                result.push(elems[i].dataset.filterValue);
            }
        }
        return result;
    };

    var applyTo = function (tag, className, filter) {
        var elements = document.getElementsByTagName(tag);
        for (var i = elements.length - 1; i >= 0; i--) {
            if (elements[i].classList.contains(className)) {
                filter(elements[i]);
            }
        }
    };

    var statusFilterValues = getFilterValues('statusFilter');
    var categoryFilterValues = getFilterValues('categoryFilter');

    var statusFilter = function (scenario) {
        for (var i = statusFilterValues.length - 1; i >= 0; i--) {
            if (scenario.classList.contains(statusFilterValues[i]))
                return true;
        }
        return false;
    };

    var categoryFilter = null;

    if (categoryFilterValues[0] === 'all')
        categoryFilter = function (scenario) { return true; }
    else if (categoryFilterValues[0] === 'without')
        categoryFilter = function (scenario) { return scenario.dataset.categories == ''; }
    else categoryFilter = function (scenario) {
        for (var i = categoryFilterValues.length - 1; i >= 0; i--) {
            if (scenario.dataset.categories.indexOf(categoryFilterValues[i]) >= 0)
                return true;
        }
        return false;
    };

    var childFilter = function (feature) {
        var children = feature.getElementsByTagName('div');
        for (var i = children.length - 1; i >= 0; i--) {
            if (children[i].classList.contains('scenario')) {
                if (children[i].dataset.filtered == 'true')
                    return true;
            }
        }
        return false;
    };

    applyTo('div', 'scenario', function (scenario) {
        scenario.dataset.filtered = statusFilter(scenario) && categoryFilter(scenario);
        scenario.className = scenario.className; //IE fix
    });
    applyTo('article', 'feature', function(feature) {
        feature.dataset.filtered = childFilter(feature);
        feature.className = feature.className; //IE fix
    });
}