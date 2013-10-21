var first = true;
var pdfDoc = null,
    pageNum = 1,
        scale = 1,
        canvas = document.getElementById('canvas'),
        ctx = canvas.getContext('2d');

//
// Get page info from document, resize canvas accordingly, and render page
//
function renderPage(num) {
    // Using promise to fetch the page
    pdfDoc.getPage(num).then(function (page) {
        var viewport = page.getViewport(scale);
        canvas.height = viewport.height;
        canvas.width = viewport.width;

        if (first) {
            $("#overlay").prop('height', $("#canvas").height());
            $("#overlay").prop('width', $("#canvas").width());
            first = false;
        }

        // Render PDF page into canvas context
        var renderContext = {
            canvasContext: ctx,
            viewport: viewport
        };
        page.render(renderContext);
    });

    // Update page counters
    document.getElementById('page_num').textContent = pageNum;
    document.getElementById('page_count').textContent = pdfDoc.numPages;
}

//
// Go to previous page
//
$("#previous-button").click(function () {
    if (pageNum <= 1)
        return;
    pageNum--;
    renderPage(pageNum);
});

//
// Go to next page
//
$("#next-button").click(function () {
    if (pageNum >= pdfDoc.numPages)
        return;
    pageNum++;
    renderPage(pageNum);
});

//
// Asynchronously download PDF as an ArrayBuffer
//
PDFJS.getDocument(fileLink).then(function getPdfHelloWorld(_pdfDoc) {
    pdfDoc = _pdfDoc;
    renderPage(pageNum);
});
