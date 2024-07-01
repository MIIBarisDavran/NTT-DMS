function previewDocument(url) {
    console.log('Preview URL:', url);  // Log the URL to check if it's correct
    var previewArea = document.getElementById('previewArea');
    previewArea.innerHTML = ''; // Clear previous content

    fetch(url)
        .then(response => {
            console.log('Fetch response:', response);  // Log the fetch response
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.blob();
        })
        .then(blob => {
            console.log('Blob:', blob);  // Log the blob to inspect it
            var fileType = blob.type;

            if (fileType.includes('image')) {
                var img = document.createElement('img');
                img.src = URL.createObjectURL(blob);
                previewArea.appendChild(img);
            } else if (fileType.includes('pdf') || fileType.includes('docx')) {
                var iframe = document.createElement('iframe');
                iframe.src = URL.createObjectURL(blob);
                previewArea.appendChild(iframe);
            } else if (fileType.includes('text') || fileType.includes('plain')) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var textContent = document.createElement('pre');
                    textContent.textContent = e.target.result;
                    previewArea.appendChild(textContent);
                };
                reader.readAsText(blob);
            } else if (fileType.includes('spreadsheetml') || fileType.includes('excel')) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var data = new Uint8Array(e.target.result);
                    var workbook = XLSX.read(data, { type: 'array' });
                    var html = XLSX.write(workbook, { type: 'string', bookType: 'html' });
                    previewArea.innerHTML = html;
                };
                reader.readAsArrayBuffer(blob);
            } else if (fileType.includes('wordprocessingml')) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    mammoth.convertToHtml({ arrayBuffer: e.target.result })
                        .then(displayResult)
                        .catch(handleError);
                };
                reader.readAsArrayBuffer(blob);
            } else {
                previewArea.textContent = 'Unsupported file type.';
            }

            function displayResult(result) {
                previewArea.innerHTML = result.value;
            }

            function handleError(err) {
                previewArea.textContent = 'Error loading document preview.';
                console.error('Error:', err);
            }

            // Show the modal after loading content
            var documentPreviewModal = new bootstrap.Modal(document.getElementById('documentPreviewModal'));
            documentPreviewModal.show();
        })
        .catch(error => {
            previewArea.textContent = 'Error loading document preview.';
            console.error('Error:', error);
        });
}

function showValue(value) {
    var previewArea = document.getElementById('previewArea');
    previewArea.innerHTML = ''; // Clear previous content
    document.getElementById('previewArea').innerText = JSON.stringify(value);
    $('#valueModal').modal('show');
}

function showOriginalValue(button) {
    var value = button.getAttribute('data-original-value');
    var previewArea = document.getElementById('previewArea');
    previewArea.innerHTML = ''; // Clear previous content
    previewArea.innerText = value;
    $('#valueModal').modal('show');
}

function showNewValue(button) {
    var value = button.getAttribute('data-new-value');
    var previewArea = document.getElementById('previewArea');
    previewArea.innerHTML = ''; // Clear previous content
    previewArea.innerText = value;
    $('#valueModal').modal('show');
}