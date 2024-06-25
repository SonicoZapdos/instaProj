
{
    let selectedFiles = [];

    function handleFileSelect(event) {
        const files = event.target.files;
        for (let i = 0; i < files.length; i++) {
            selectedFiles.push(files[i]);
        }
        displaySelectedFiles();
    }

    function stringSelect() {
        selectedFiles.push(document.getElementById('linkInput').value);
        displaySelectedFiles();
    }

    function displaySelectedFiles() {
        const carouselInner = document.getElementById('carouselInner');
        carouselInner.innerHTML = '';

        selectedFiles.forEach((file, index) => {
            if (typeof file !== 'string') {
                const reader = new FileReader();
                reader.onload = function (e) {
                    const item = document.createElement('div');
                    item.className = 'carousel-item' + (index === 0 ? ' active' : '');
                    item.innerHTML = `
                                                        <img src="${e.target.result}" class="maxW" style="height: 300px" alt="Imagem Selecionada">
                                                            <button type="button" class="remove-btn z-3" onclick="removeFile(${index})"><span class="material-symbols-outlined no-config">
                                close
                                </span></button>
                                                    `;
                    carouselInner.appendChild(item);
                    document.getElementById('carouselExampleControls').classList.remove('d-none');
                };
                reader.readAsDataURL(file);
            }
            else {
                const item = document.createElement('div');
                var linkShow = 'https://img.youtube.com/vi/' + file.substring(32) + '/0.jpg';
                item.className = 'carousel-item' + (index === 0 ? ' active' : '');
                item.innerHTML = `
                                                                    <img src="${linkShow}" class="maxW" style="height: 300px" alt="Imagem Selecionada">
                                                                <button type="button" class="remove-btn z-3" onclick="removeFile(${index})"><span class="material-symbols-outlined no-config">
                                    close
                                    </span></button>
                                                        `;
                carouselInner.appendChild(item);
                document.getElementById('carouselExampleControls').classList.remove('d-none');
            }
        });
    }

    function removeFile(index) {
        selectedFiles.splice(index, 1);
        if (selectedFiles.length == 0) {
            document.getElementById('carouselExampleControls').classList.add('d-none');
        }
        displaySelectedFiles();
    }

    document.getElementById('postForm').addEventListener('submit', function (event) {
        event.preventDefault(); // Evitar o envio automático do formulário

        const description = document.getElementById('floatingTextarea').value.trim();
        const errorMessage = document.getElementById('error-message');

        if (description === "" && selectedFiles.length === 0) {
            errorMessage.style.display = 'block';
            return;
        } else {
            errorMessage.style.display = 'none';
        }

        const formData = new FormData(this);
        selectedFiles.forEach((file, index) => {

            if (typeof file !== 'string') {
                formData.append('Archives', file); // Adicionar cada arquivo ao FormData
            } else {
                formData.append('Links', file); // Adicionar cada link ao FormData
            }
        });

        fetch('@Url.Action("CreatePost", "Posts")', {
            method: 'POST',
            body: formData,
            headers: {
                'X-CSRF-TOKEN': $('input[name="__RequestVerificationToken"]').val() // CSRF token
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    console.log('Post enviado com sucesso:', data);
                    window.location.href = data.redirectUrl; // Redireciona para a URL de sucesso
                } else {
                    console.error('Erro ao enviar o post.');
                    window.location.href = data.redirectUrl; // Redireciona para a URL de falha, se desejado
                }
            })
            .catch(error => {
                console.error('Erro:', error);
                // Tratar o erro conforme necessário
            });
    });
} /* Carrossel */

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
