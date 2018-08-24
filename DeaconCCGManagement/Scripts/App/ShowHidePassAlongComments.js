//
// Shows or hides the pass along comments determined by check box
//
$(function () {
    $("#chkbxPassAlong").change(function () {
        var checkedValue = $("#chkbxPassAlong:checked").val();
        var msgPassAlong = $("#msgPassAlong");
        var msgPassAlongTextArea = $("#msgPassAlongTextArea");
       
        if (checkedValue) {
            msgPassAlong.show();
            msgPassAlongTextArea.focus();
        } else {
            msgPassAlong.hide();
        }
    });
});