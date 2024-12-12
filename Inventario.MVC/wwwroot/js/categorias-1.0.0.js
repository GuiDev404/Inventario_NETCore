function clearForm (){
  // Limpia campos del formulario
  const form_modal = $("#modal #formModal");
  form_modal[0].reset()
  $("#modal #formModal input[type=hidden]").val(0);

  // Limpia los errores y validaciones
  const validator = form_modal.data('validator');
  validator.resetForm();
}

function mostrarCategorias (){
  const cantidad = $('#cantidad_text')
  const t_body = $('#tBody')
  const t_footer_first_row = $('#tFooter > tr > td')
  
  $.ajax({
    url: '/Categorias/GetCategorias',
    type: 'GET',
    dataType: 'json',
    success: function (response, _status, _response){
      t_body.empty();

      if (response.success){
              
        const noItems = response && response.data.length === 0

        if(noItems){
          cantidad.text('No hay categorias');
  
          t_footer_first_row
            .removeClass('d-none')
            .addClass('text-center')
            .html(`
              <i class="fa fa-th-large fs-1 mt-4"></i>
              <p> 
                No se encontraron categorias.
                <button type="button" class="btn btn-link p-0 " data-bs-toggle="modal" data-bs-target="#modal">
                  Agregar uno.
                </button>
              </p>
            `)
          
          return;
        }
        
        cantidad.text(`${response.data.length} categorias`);

        response.data.forEach(categoria=> {
          let acciones = ''
          const classesCellDeshabilitada = categoria.eliminado ? 'opacity-50 text-decoration-line-through' : ''

          if(categoria.eliminado){
            acciones = `
              <td class="text-end">
                <button type="button" class="btn btn-success btn-sm" onclick="eliminarCategoria(${categoria.id}, false)">
                  <i class="fas fa-check small"></i>
                  Habilitar
                </button>
              </td>
            `
          } else {
            acciones = `
              <td class="text-end">
                <button type="button" class="btn btn-primary btn-sm" onclick="buscarCategoria(${categoria.id})">
                  <i class='bx bx-edit'></i>
                  <span class="d-none d-md-inline-block text-uppercase small"> Editar </span>
                </button>
                <button type="button" class="btn btn-danger btn-sm" onclick="eliminarCategoria(${categoria.id})">
                  <i class='bx bx-trash'></i>
                  <span class="d-none d-md-inline-block text-uppercase small"> Deshabilitar </span>
                </button>
              </td>
            `
          }

          t_body.append(`
            <tr class="">
              <td class="${classesCellDeshabilitada}">
                 ${categoria.nombre}
              </td>
           
              ${acciones}
            </tr>
          `);
        })
      }

    },
    error: function (error){
      console.error({ error });
    }
  })
}

function eliminarCategoria (id){
  $.ajax({
    url: '/Categorias/DeleteCategoria/'+id,
    type: 'POST',
    dataType: 'json',
    success: function (data, status, response){
     if (response.status === 204){
       mostrarCategorias()
       toast({ type: 'success', message: 'Categoria deshabilitada correctamente', title: 'Deshabilitar categoria' })
     } 
    },
    error: function (error){
      
      toast({ 
        type: 'error',
        message: error?.responseJSON?.message || 'No se pudo deshabilitar la categoria',
        title: 'Deshabilitar categoria'
      })
    },
  })
}

function buscarCategoria (id){
  $.ajax({
    url: '/Categorias/GetCategoriaById/'+id,
    type: 'GET',
    dataType: 'json',
    success: function (response){

      if(response.success){
        const categoria = response.data;

        $('#categoriaId').val(categoria.id)
        $('#Nombre').val(categoria.nombre)

        $("#modal").modal('show');
      } else {
        toast({ type: 'error', title: 'Busqueda de categoria', message: response.message });
      }
    },
    error: function (error){
      if (error.status === 404){
        toast({ type: 'error', title: 'Busqueda de categoria', message: error.responseJSON.message })
        return;
      } 
    },
  })
}

function guardarCategoria (){
  const categoriaId = $('#categoriaId').val()
  const nombre = $('#Nombre').val()

  if(!$("#modal #formModal").valid()) return;

  if (categoriaId === '0'){
    console.log('CREATE: ',{ categoriaId, nombre });

    $.ajax({
      url: '/Categorias/CreateCategoria',
      data: { Nombre: nombre },
      type: 'POST',
      dataType: 'json',
      success: function (response){
        console.log(response);
        mostrarCategorias()
        toast({ type: 'success', title: 'Creacion categoria', message: 'Categoria creada correctamente' })
        $("#modal").modal('hide');
      },
      error: function (error){
        if (error.status === 400 && error.responseJSON !== null){
          error.responseJSON.data.forEach(syncBackendError)
        } else {
          toast({ type: 'error', title: 'Creacion categoria', message: error.responseJSON.message })
        }
  
      }
    })

  } else {
   
    $.ajax({
      url: '/Categorias/EditCategoria/'+categoriaId,
      data: { Nombre: nombre, Id: categoriaId },
      type: 'POST',
      dataType: 'json',
      success: function (response){

        toast({ type: 'success', title: 'Edicion categoria', message: response.message })
        mostrarCategorias()
        $("#modal").modal('hide');
      },
      error: function (error){
        if (error.status === 400 && error.responseJSON !== null){
          console.log(error.responseJSON.data);
          return;
        } else {
          toast({ type: 'error', title: 'Edicion categoria', message: error.responseJSON.message })
        }
      }
    })

  }
}

$(document).ready(function() {
  $("#modal").on('hide.bs.modal', function(){
    clearForm()
  });
  
  $("#modal").on('show.bs.modal', function(){
    const tituloModal = document.getElementById('modalTitulo')
    const id = $("#categoriaId").val();

    tituloModal.innerText = id === '0' ? 'Agregar categoria' : 'Actualizar categoria'
  });
  
  $('#modal #formModal').validate({
    errorClass: 'text-danger small d-block mt-1',
    rules: {
      Nombre: {
        required: true,
        maxlength: 100
      }
  
    },
    messages: {
      Nombre: {
        required: "El nombre de la categoria es requerida",
        maxlength: jQuery.validator.format("La categoria debe tener menos de {0} characteres")
      },
  
    },
  })
  
  const inputBuscador = document.getElementById('buscador')
  inputBuscador.addEventListener('input', filtrarPorBusqueda)
  
  const TIEMPO_DE_ESPERA = 700;
  setTimeout(function () {
    mostrarCategorias()
  }, TIEMPO_DE_ESPERA)

})
