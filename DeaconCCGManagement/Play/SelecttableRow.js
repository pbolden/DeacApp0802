// 6-30-17
// Played around with selecting a row
// of contact records and redirecting to details view
//


$(function () {
    // send user to details view if row is clicked
    $('table').click(function (e) {

        // select the closest 'td'
        var $td = $(e.target).closest('td');
        var td = $td[0];

        // ignore if user clicks on the drop down list
        if (td.className === "dropdownlist") {
            return;
        }
        // 'tr' id is the same as the record id
        // use this id to pass back to controller
        var $tr = $(e.target).closest('tr');
        var id = $tr[0].id;
        $.redirect("localhost:63896");
    });


    $('#btnTest').click(function (e) {
        $.get("localhost:63896");
    });
});
