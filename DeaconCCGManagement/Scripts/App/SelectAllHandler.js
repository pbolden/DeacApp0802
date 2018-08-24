/* when select/deselect all checkbox is changed, update the
   label text and check or uncheck all other check boxes.
   also enable or disable the other check boxes accordingly */

$(function () {

    var deselctAllTxt = "Deselect All";

    var selectAll = function () {
        $('.chkbxSelect').attr('checked', 'checked');
        $(".chkbxSelect").prop({ "disabled": true });
    };

    /* on page load if 'selectAll' is true, check all check boxes */
    var isSelectAll = $('#selectAllHidden').val() === "True";

    if (isSelectAll) {
        $("#chkboxToggle").attr("checked", "checked");
        $("#selectAllText").text(deselctAllTxt);
        selectAll();
    }

    /* if select all check box change update hidden element value
       and submit select all form*/
    $("#chkboxToggle").on("change", function () {
        if ($("#chkboxToggle:checked").val()) {
            $("#selectAll2").val(true);
            $("#frmSelectAll").submit();
        } else {
            $("#selectAll2").val(false);
            $("#frmSelectAll").submit();
        }
    });
});