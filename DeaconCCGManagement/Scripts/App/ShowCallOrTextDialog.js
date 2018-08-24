//
// Opens modal dialog when button of .openDialog class is clicked
//
$(function () {
    $('.openDialog').on('click', function () {
        $('#callOrTextDialog').modal();
    });
});
