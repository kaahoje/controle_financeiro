function abrirModal(url, titulo) {
    $('#mainModalLabel').text(titulo);
    $('#mainModalBody').html('<div class="text-center"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');
    $('#mainModal').modal('show');

    $.get(url, function (data) {
        $('#mainModalBody').html(data);
        bindFormEvents();
    });
}

function bindFormEvents() {
    $('#mainModalBody form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var url = form.attr('action');
        var formData = new FormData(this);

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                // Se a resposta for um objeto JSON com sucesso
                if (response.success) {
                    $('#mainModal').modal('hide');
                    if (typeof Filtrar === 'function') {
                        Filtrar();
                    } else {
                        location.reload();
                    }
                } else {
                    // Se falhar (validação), o controller deve retornar o PartialView com os erros
                    $('#mainModalBody').html(response);
                    bindFormEvents();
                }
            },
            error: function (xhr) {
                // Se o retorno for HTML (erro de validação), substitui o conteúdo
                if (xhr.status === 400 || xhr.status === 200) {
                    $('#mainModalBody').html(xhr.responseText);
                    bindFormEvents();
                } else {
                    alert('Ocorreu um erro ao salvar os dados.');
                }
            }
        });
    });
}

$(document).ready(function () {
    // Intercepta cliques em botões/links que devem abrir em modal
    $(document).on('click', '.btn-modal', function (e) {
        e.preventDefault();
        var url = $(this).attr('href') || $(this).data('url');
        var titulo = $(this).attr('title') || $(this).data('titulo') || 'Cadastro';
        abrirModal(url, titulo);
    });
});
