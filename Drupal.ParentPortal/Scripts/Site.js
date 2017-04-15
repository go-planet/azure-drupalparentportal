var hostUriPrefix = "";

function GetGuid(uri, stripDashes) {
    var d = jQuery.Deferred();
    $.ajax({
        type: 'GET',
        async: false,
        url: uri,
        cache: false,
        dataType: 'json',
        success: function (data) {
            // alert("Success!");
        },
        error: function (e, r, s) {
            alert('error getting guid');
        }
    }).done(function (data) {
        if (stripDashes === true) {
            d.resolve(data.replace(/-/g, ''));
        }
        else {
            d.resolve(data);
        }
    });

    return d.promise();
}

function GetRandomString(length) {
    var s = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_";
    var randomString = Array.apply(null, Array(length)).map(function () {
        return s.charAt(Math.floor(Math.random() * s.length));
    }).join('');

    return randomString;
}
