/*  Iterate through all check boxes and add value to array if checked.
    The value is the member's id. From this array, append hidden input
    elements inside a div in the 'frmContactSelected' form.
    Lastly, submit the form.*/

$(function () {
    $('#btnBulkContact').on("click", function () {

        // clear div section
        $("#divMemberIds").html("");

        // push selected member ids to array (member id is the checkbox's value)
        var membersSelected = null;
        membersSelected = [];
        $('.chkbxSelect:checked').each(function () {
            membersSelected.push($(this).attr('value'));
        });

        // add hidden input elements in form (frmContactSelected) for each member checked
        // uses member ids for values
        var newHiddenInput;
        for (var i = 0; i < membersSelected.length; i++) {
            newHiddenInput = "<input name=\"memberIds\" hidden=\"hidden\" value=\"" + membersSelected[i] + "\"/>";
            $("#divMemberIds").append(newHiddenInput);
        }

        // update selectAll value in form's hidden input element
        if ($("#chkboxToggle:checked").val()) {
            $("#selectAll").val(true);
        } else {
            $("#selectAll").val(false);
        }

        // submit form
        $("#frmContactSelected").submit();
    });
});