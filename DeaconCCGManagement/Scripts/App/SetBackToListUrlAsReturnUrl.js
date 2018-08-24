/*
 * Assign back-to-list url to hidden input element
 * The url gets assigned to the return property in the view models
 */

$(function () {
    var backToListUrl = $("#backToListLink").attr("href");
    $("#ReturnUrl").attr("value", backToListUrl);
});