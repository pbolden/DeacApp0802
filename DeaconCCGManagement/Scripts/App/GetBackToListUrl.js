//
// Gets url from browser storage and updates 'Back to List' links
//

$(function() {

    var storage = Storages.sessionStorage;

    var key = "backToListUrl";

    if (!storage.isEmpty(key)) {
        var url = storage.get(key);
        $("#backToListLink").attr("href", url);
    } 
});