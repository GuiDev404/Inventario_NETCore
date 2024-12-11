function clearFormMovimientos (){
  // Limpia campos del formulario
  const form_modal = $("#modalMovimientos #formModal");
  form_modal[0].reset()
  $("#movimientoId").val(0);
  
  $('#stockActual').val("");
  $('#movimientoProductoId').val("");
  $('#TipoMovimiento').val("");

  // Limpia los errores y validaciones
  const validator = form_modal.data('validator');
  validator.resetForm();
}

function nuevoMovimiento (productoId, tipoMovimiento, stockActual){
  $('#modalMovimientos').modal('show');
    
  $('#stockActual').val(stockActual);
  $('#movimientoProductoId').val(productoId);
  $('#TipoMovimiento').val(tipoMovimiento);
}

function guardarMovimiento (event){
  console.log(event);
  const cantidad = $("#Cantidad").val()
  const productoId = $("#movimientoProductoId").val()
  const tipoMovimiento = $("#TipoMovimiento").val()
  const stockActual = $('#stockActual').val();

  // if(!$("#modalMovimientos #formModal").valid()) return;
  
  // if (Number(tipoMovimiento) === 1 && Number(cantidad) > Number(stockActual)){
  //   $('#errorResumen').removeClass("d-none")
  //   $('#errorResumen span').text('La cantidad de salida debe ser menor que el stock actual')
      
  //   setTimeout(() => {
  //     $('#errorResumen').addClass("d-none")
  //     $('#errorResumen span').text('')
  //   }, 3000);
  //   return 
  // }

  $.ajax({
    url: '/Movimientos/NuevoMovimiento',
    data: {
      TipoMovimiento: tipoMovimiento,
      Cantidad: cantidad,
      ProductoID: productoId
    },
    type: 'POST',
    dataType: 'json',
    success: function (response){
      console.log(response);
      mostrarProductos()
      toast({ type: 'success', title: 'Movimiento de producto', message: 'Movimiento realizado correctamente' })
      $("#modalMovimientos").modal('hide');
    },
    error: function (error){
      
      if (error.status === 400 && error?.responseJSON?.data !== null){
        error.responseJSON.data.forEach(syncBackendError);
        return;
      } else {

        toast({ 
          type: 'error',
          title: 'Movimiento de producto',
          message: error?.responseJSON?.message || 'Algo salio mal'
        })
      }

    }
  })

}

$(document).ready(function() {
  $("#modalMovimientos").on('hide.bs.modal', function(){
    clearFormMovimientos()
  });
  
  // $("#modalMovimientos").on('show.bs.modal', function(){
  //   const tituloModal = document.getElementById('modalTitulo')
  //   const id = $("#movimientoId").val();

  //   tituloModal.innerText = id === '0' ? 'Agregar movimiento' : 'Actualizar movimiento'
  // });
  
  $('#modalMovimientos #formModal').validate({
    errorClass: 'text-danger small d-block mt-1',
    rules: {
      Cantidad: {
        required: true,
        digits: true,
        min: 1
      },
      // tipoMovimiento: {
      //   required: true,
      //   digits: true,
      //   min: 0
      // },
    },
    messages: {
      Cantidad: {
        required: "La cantidad del movimiento es requerida",
        digits: 'Ingrese una cantidad entera',
        min: "Ingrese una cantidad para el movimiento"
        // min: jQuery.validator.format("El movimiento debe tener un entrada/salida minimo de {0}")
      },
      // tipoMovimiento: {
      //   required: "El tipo de movimiento es requerido",
      //   digits: "Seleccione un tipo de movimiento",
      //   min: "Seleccione un tipo de movimiento"
      // },
    },
  })

})
