//
// Implements JS Storage to store 'Back to List' url.
//

$(function() {

    var storage = Storages.sessionStorage;

    var key = "backToListUrl";

    // store 'back to list' url in browser's session storage
    storage.set(key, document.URL);
});
