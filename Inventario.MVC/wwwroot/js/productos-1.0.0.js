const zoom = mediumZoom({
  background: '#262626e0',
})

function clearForm (){
  // Limpia campos del formulario
  const form_modal = $("#modal #formModal");
  form_modal[0].reset()
  $("#modal #formModal input[type=hidden]").val(0);

  // Limpia los errores y validaciones
  const validator = form_modal.data('validator');
  validator.resetForm();
}

function mostrarProductos (){
  const cantidad = $('#cantidad_text')
  const t_body = $('#tBody')
  const t_footer_first_row = $('#tFooter > tr > td')
  
  $.ajax({
    url: '/Productos/GetProducts',
    type: 'GET',
    dataType: 'json',
    success: function (response, _status, _response){
      t_body.empty();

      if (response.success){
              
        const noItems = response && response.data.length === 0

        if(noItems){
          cantidad.text('No hay productos');
  
          t_footer_first_row
            .removeClass('d-none')
            .addClass('text-center')
            .html(`
              <i class="fa fa-th-large fs-1 mt-4"></i>
              <p> 
                No se encontraron productos.
                <button type="button" class="btn btn-link p-0 " data-bs-toggle="modal" data-bs-target="#modal">
                  Agregar uno.
                </button>
              </p>
            `)
          
          return;
        }
        
        cantidad.text(`${response.data.length} productos`);

        response.data.forEach(producto=> {
          let acciones = ''
          const classesCellDeshabilitada = producto.eliminado ? 'opacity-50 text-decoration-line-through' : ''

          if(producto.eliminado){
            acciones = `
              <td class="text-end">
                <button type="button" class="btn btn-success btn-sm" onclick="eliminarProducto(${producto.id}, false)">
                  <i class="fas fa-check small"></i>
                  Habilitar
                </button>
              </td>
            `
          } else {
            acciones = `
              <td class="text-end">
                <button type="button" class="btn btn-outline-secondary btn-sm" onclick="nuevoMovimiento(${producto.id}, 0, ${producto.cantidad})">
                  <i class='bx bx-list-plus'></i>
                </button>
                
                <button type="button" class="btn btn-outline-secondary btn-sm" onclick="nuevoMovimiento(${producto.id}, 1, ${producto.cantidad})"  ${producto.cantidad === 0 ? 'disabled' : ''}>
                  <i class='bx bx-list-minus' ></i>
                </button>

                <button type="button" class="btn btn-primary btn-sm" onclick="buscarProducto(${producto.id})">
                  <i class='bx bx-edit'></i>
                  <span class="d-none d-md-inline-block text-uppercase small"> Editar </span>
                </button>
                <button type="button" class="btn btn-danger btn-sm" onclick="eliminarProducto(${producto.id}, true)">
                  <i class='bx bx-trash'></i>
                  <span class="d-none d-md-inline-block text-uppercase small"> Deshabilitar </span>
                </button>
              </td>
            `
          }

          t_body.append(`
            <tr class="">
              <td class="ocultar-sm text-center ${classesCellDeshabilitada}">
                <img src="${producto.imagenUrl || './imgs/placeholder_img.jpg'}" width="75" class="rounded-2" alt="${producto.descripcion}" title="${producto.descripcion}" data-zoomable />
                <p class="${classesCellDeshabilitada} text-muted flex align-items-center" style="font-size: .8rem;">
                  <i class='bx bx-barcode'></i>
                  <span class="">  ${producto.codigoBarra} </span>
                </p>
              </td>
              <td class="${classesCellDeshabilitada}">
                 ${producto.nombre}
              </td>
              <td class="ocultar-sm ${classesCellDeshabilitada}">
                 ${producto.descripcion}
              </td>
              <td class="ocultar-md ${classesCellDeshabilitada}">
                ${producto.precio}
              </td>
              <td class="ocultar-md ${classesCellDeshabilitada}">
                ${producto.cantidad}
              </td>
              <td class="ocultar-md ${classesCellDeshabilitada}">
                ${producto.categoria.nombre}
              </td>

              ${acciones}
            </tr>
          `);
        })
      }

      zoom.attach('[data-zoomable]')
    },
    error: function (error){
      console.error({ error });
    }
  })
}

function eliminarProducto (id){
  $.ajax({
    url: '/Productos/DeleteProduct/'+id,
    type: 'POST',
    dataType: 'json',
    success: function (data, status, response){
     if (response.status === 204){
       mostrarProductos()
       toast({ type: 'success', message: 'Producto deshabilitado correctamente', title: 'Deshabilitar producto' })
     } 
    },
    error: function (error){
      
      toast({ 
        type: 'error',
        message: error?.responseJSON?.message || 'No se pudo deshabilitar el producto',
        title: 'Deshabilitar producto'
      })
    },
  })
}

function buscarProducto (id){
  $.ajax({
    url: '/Productos/GetProductById/'+id,
    type: 'GET',
    dataType: 'json',
    success: function (response){

      if(response.success){
        const producto = response.data;
        console.log(producto);
        $('#productoId').val(producto.id)
        $('#Nombre').val(producto.nombre)
        $('#Descripcion').val(producto.descripcion)
        // $('#cantidad').val(producto.cantidad)
        $('#Precio').val(producto.precio)
        $('#CodigoBarra').val(producto.codigoBarra)
        $('#CategoriaID').val(producto.categoriaID)
        $('#ImagenUrl').val(producto.imagenUrl || '')
        
        $("#modal").modal('show');
      } else {
        toast({ type: 'error', title: 'Editar producto', message: response.message });
      }
    },
    error: function (error){
      if (error.status === 404){
        toast({ type: 'error', title: 'Busqueda de producto', message: error.responseJSON.message })
        return;
      } 
    },
  })
}

function guardarProducto (){
  const form_modal = $("#modal #formModal")[0];
  const data = new FormData(form_modal)

  const producto = Object.fromEntries(data.entries());
  producto.CodigoBarra = $('#CodigoBarra').val()

  if(!$("#modal #formModal").valid()) return;

  if (producto.id === '0'){
    
    console.log(producto);
    $.ajax({
      url: '/Productos/CreateProduct',
      data: producto,
      type: 'POST',
      dataType: 'json',
      success: function (response){
        console.log(response);
        mostrarProductos()
        toast({ type: 'success', title: 'Creacion producto', message: 'Producto creado correctamente' })
        $("#modal").modal('hide');
      },
      error: function (error){
        if (error.status === 400 && error.responseJSON !== null){
          error.responseJSON.data.forEach(syncBackendError)
        } else {
          toast({ type: 'error', title: 'Creacion producto', message: error.responseJSON.message })
        }
  
      }
    })

  } else {
    $.ajax({
      url: '/Productos/EditProduct/'+producto.id,
      data: producto,
      type: 'POST',
      dataType: 'json',
      success: function (response){

        toast({ type: 'success', title: 'Edicion producto', message: response.message })
        mostrarProductos()
        $("#modal").modal('hide');
      },
      error: function (error){
        if (error.status === 400 && error.responseJSON !== null){
          console.log(error.responseJSON.data);
          return;
        } else {
          toast({ type: 'error', title: 'Edicion producto', message: error.responseJSON.message })
        }
  
      }
    })
  }
}

// EVENTOS E INICIALIZACION

$(document).ready(function() {
  $('#modalCodigoBarra').on('show.bs.modal', () => {
    
    $('#modal').css('z-index', 'auto')
  });
  
  $('#modalCodigoBarra').on('hide.bs.modal', () => {
    
    $('#modal').css('z-index', '')
  });

  $("#modal").on('hide.bs.modal', function(event){
    clearForm()
  });
  
  $("#modal").on('show.bs.modal', function(){
    const tituloModal = document.getElementById('modalTitulo')
    const id = $("#productoId").val();

    tituloModal.innerText = id === '0' ? 'Agregar producto' : 'Actualizar producto'
  });
  
  $('#modal #formModal').validate({
    errorClass: 'text-danger small d-block mt-1',
    rules: {
      Nombre: {
        required: true,
        maxlength: 100
      },
      Descripcion: {
        required: true,
        maxlength: 500
      },
      Precio: {
        required: true,
        number: true
      },
      CodigoBarra: {
        required: true,
      },
      CategoriaID: {
        required: true,
        digits: true,
        min: 1
      }
    },
    messages: {
      Nombre: {
        required: "El nombre del producto es requerido",
        maxlength: jQuery.validator.format("El producto debe tener menos de {0} characteres")
      },
      Descripcion: {
        required: "La descripcion del producto es requerida",
        maxlength: jQuery.validator.format("El producto debe tener menos de {0} characteres")
      },
      Precio: {
        required: "El precio del producto es requerido",
        number: 'Ingrese un precio valido',
        min: jQuery.validator.format("El producto debe tener un precio minimo de ${0}")
      },
      CodigoBarra: {
        required: "El codigo de barra es requerido",
      },
      CategoriaID: {
        required: "La categoria es requerida",
        digits: "Seleccione una categoria",
        min: "Seleccione una categoria"
      }
    },
  })
  
  const inputBuscador = document.getElementById('buscador')
  inputBuscador.addEventListener('input', filtrarPorBusqueda)
  
  const TIEMPO_DE_ESPERA = 700;
  setTimeout(function () {
    mostrarProductos()
  }, TIEMPO_DE_ESPERA)

})

