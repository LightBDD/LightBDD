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