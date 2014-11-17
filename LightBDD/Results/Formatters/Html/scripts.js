function checkAll(checkBoxClass, checked) {
    var buttons = document.getElementsByTagName('input');
    for (var i = buttons.length - 1; i >= 0; i--) {
        if (buttons[i].classList.contains(checkBoxClass))
            buttons[i].checked = checked;
    }
}