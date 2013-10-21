// Method to put an element in the center of the screen
// Used for the overlay box that comes up after the use chose a file to upload
jQuery.fn.center = function () {
    this.css("position", "absolute");
    this.css("top", Math.max(0, (($(window).height() - $(this).outerHeight()) / 2) +
                                                $(window).scrollTop()) + "px");
    this.css("left", Math.max(0, (($(window).width() - $(this).outerWidth()) / 2) +
                                                $(window).scrollLeft()) + "px");
    return this;
}

// Handles the file upload
$('#fileupload').fileupload({
    add: function (e, data) {
        var goUpload = true;
        var uploadFile = data.files[0];
        if (!(/\.(pdf)$/i).test(uploadFile.name)) {
            alert("Invalid file type. Only PDF files are accepted!");
            goUpload = false;
        }
        if (goUpload == true) {
            data.submit();
        }
    },
    type: 'post',
    done: function (e, data) {
        var jsonArray = JSON.parse(data.result);
        if (jsonArray["statusCode"] == 200) {
            window.location.href = '/Process';
        } else {
            alert("Wrong File Format!");
        }
    },
    progressall: function (e, data) {
        // When the progress has been started,
        // If there is no overlay yet, create one.
        if ($(".overlay").length == 0) {
            // Adding an overlay
            $('<div class="overlay"></div>').appendTo($("body"));
            // Adding an overlay-info screen for the user
            $('<div class="overlay-info">' +
                    '<div id="progress">' +
                        '<div class="bar" style="width: 0%;"></div>' +
                    '</div>' +
                '</div>').appendTo($("body"));

            $(".overlay-info").center();
        }

        // Handle the progress bar
        var progress = parseInt(data.loaded / data.total * 100, 10);
        $('#progress .bar').css(
            'width',
            progress + '%'
        );

        // If the progress bar reaches 100, display another message for the user
        if (progress == 100) {
            $('<div class="overlay-status">Please wait. Processing your image...<br /><br />' +
                'This may take up to a few minutes depending on the size of your document.</div>' +
                '<div class="loading-image">' +
               '<img src="/images/loading.gif" />' +
           '</div></div>').appendTo(".overlay-info");

            // Putting the overlay in the center of the screen again, since the size of overlay has been changed.
            $(".overlay-info").center();
        }
    }
});

// If the screen is resized, put the overlay-info box in the center of the screen again
$(window).resize(function () {
    if ((".overlay-info").length != 0) {
        $(".overlay-info").center();
    }
});