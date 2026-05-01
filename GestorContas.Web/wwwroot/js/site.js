function abrirModal(url, titulo, tamanho = '') {
    $('#mainModalLabel').text(titulo);
    $('#mainModalBody').html('<div class="text-center"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>');
    
    // Reset e aplica tamanho
    $('#mainModal .modal-dialog').removeClass('modal-sm modal-lg modal-xl').addClass(tamanho);
    
    $('#mainModal').modal('show');

    $.get(url, function (data) {
        $('#mainModalBody').html(data);
        bindFormEvents();
        initAutocomplete('[name="Descricao"]', '/Lancamentos/GetDescricoes');
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
                    initAutocomplete('[name="Descricao"]', '/Lancamentos/GetDescricoes');
                }
            },
            error: function (xhr) {
                // Se o retorno for HTML (erro de validação), substitui o conteúdo
                if (xhr.status === 400 || xhr.status === 200) {
                    $('#mainModalBody').html(xhr.responseText);
                    bindFormEvents();
                    initAutocomplete('[name="Descricao"]', '/Lancamentos/GetDescricoes');
                } else {
                    showToast('Ocorreu um erro ao salvar os dados.', 'Erro', 'danger');
                }
            }
        });
    });
}

function initAutocomplete(selector, url) {
    $(selector).autocomplete({
        source: function (request, response) {
            $.getJSON(url, { term: request.term }, response);
        },
        minLength: 2,
        select: function (event, ui) {
            $(this).val(ui.item.value);
            return false;
        }
    }).autocomplete("instance")._renderItem = function (ul, item) {
        return $("<li>")
            .append("<div class='d-flex align-items-center py-1'><i class='bi bi-clock-history me-2 text-muted'></i>" + item.value + "</div>")
            .appendTo(ul);
    };
}

function showToast(message, title = 'Notificação', type = 'info') {
    const toastElement = document.getElementById('liveToast');
    const toastBody = document.getElementById('toastMessage');
    const toastTitle = document.getElementById('toastTitle');
    const toastIcon = document.getElementById('toastIcon');

    toastBody.innerText = message;
    toastTitle.innerText = title;

    // Configura ícone e cor baseado no tipo
    toastIcon.className = 'bi me-2';
    toastElement.className = 'toast';

    if (type === 'success') {
        toastIcon.classList.add('bi-check-circle-fill', 'text-success');
        toastElement.classList.add('border-success');
    } else if (type === 'danger') {
        toastIcon.classList.add('bi-exclamation-triangle-fill', 'text-danger');
        toastElement.classList.add('border-danger');
    } else {
        toastIcon.classList.add('bi-info-circle-fill', 'text-primary');
        toastElement.classList.add('border-primary');
    }

    const toast = new bootstrap.Toast(toastElement);
    toast.show();
}

$(document).ready(function () {
    // Intercepta cliques em botões/links que devem abrir em modal
    $(document).on('click', '.btn-modal', function (e) {
        e.preventDefault();
        var url = $(this).attr('href') || $(this).data('url');
        var titulo = $(this).attr('title') || $(this).data('titulo') || 'Cadastro';
        var tamanho = $(this).data('tamanho') || '';
        abrirModal(url, titulo, tamanho);
    });

    // Delegar eventos de replicação para qualquer tela (caso necessário no futuro)
    $(document).on('submit', '.form-ajax-replicate', function (e) {
        e.preventDefault();
        const form = $(this);
        const confirmMsg = form.data('confirm');

        Swal.fire({
            title: 'Confirmar Replicação',
            text: confirmMsg,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#198754',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Sim, replicar!',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $.post(form.attr('action'), form.serialize(), function (response) {
                    if (response.success) {
                        if (typeof Filtrar === 'function') Filtrar();
                        else location.reload();
                        showToast('Registro replicado com sucesso!', 'Sucesso', 'success');
                    }
                }).fail(function () {
                    showToast('Erro ao replicar registro.', 'Erro', 'danger');
                });
            }
        });
    });

    initAutocomplete('[name="Descricao"]', '/Lancamentos/GetDescricoes');
});
