function onOpen() {
    var link = document.getElementById('linkShow');

    if (document.getElementById('checkLink').checked) {
        link.classList.remove('d-none');
        link.classList.add('d-flex');
    } else {
        link.classList.add('d-none');
        link.classList.remove('d-flex');
    }
}
