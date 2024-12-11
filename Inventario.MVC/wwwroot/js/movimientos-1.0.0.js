const TIPOS_MOVIMIENTOS = ['Entrada', 'Salida']

function mostrarMovimientos (){
  const cantidad = $('#cantidad_text')
  const t_body = $('#tBody')
  const t_footer_first_row = $('#tFooter > tr > td')
  
  $.ajax({
    url: '/Movimientos/GetMovimientos',
    type: 'GET',
    dataType: 'json',
    success: function (response, _status, _response){
      t_body.empty();

      if (response.success){
              
        const noItems = response && response.data.length === 0

        if(noItems){
          cantidad.text('No hay movimientos');
  
          t_footer_first_row
            .removeClass('d-none')
            .addClass('text-center')
            .html(`
              <i class="fa fa-th-large fs-1 mt-4"></i>
              <p> 
                No se realizaron movimientos
              </p>
            `)
          
          return;
        }
        
        cantidad.text(`${response.data.length} movimientos`);

        response.data.forEach(movimiento=> {
          const classesCellDeshabilitada = movimiento.eliminado ? 'opacity-50 text-decoration-line-through' : ''
          const classPorTipo = movimiento.tipoMovimiento === 0 ? 'bg-success' : 'bg-danger'

          t_body.append(`
            <tr class="${classPorTipo} bg-opacity-50">
              <td class="ocultar-sm text-center ${classesCellDeshabilitada}">
                <span class="">  ${movimiento.producto.nombre} </span>
                
                <p class="${classesCellDeshabilitada} text-muted flex align-items-center" style="font-size: .8rem;">
                  <i class='bx bx-barcode' ></i>
                  <span class="">  ${movimiento.producto.codigoBarra} </span>
                </p>
              </td>
              <td class="${classesCellDeshabilitada}">
                 ${TIPOS_MOVIMIENTOS[movimiento.tipoMovimiento]}
              </td>
              <td class="ocultar-sm ${classesCellDeshabilitada}">
                 ${movimiento.stock}
              </td>
              <td class="ocultar-md ${classesCellDeshabilitada}">
                ${movimiento.cantidad}
              </td>
              <td class="ocultar-md ${classesCellDeshabilitada}">
                ${new Date(movimiento.fecha).toLocaleString()}
              </td>

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

$(document).ready(function() {

  const inputBuscador = document.getElementById('buscador')
  inputBuscador.addEventListener('input', filtrarPorBusqueda)
  
  const TIEMPO_DE_ESPERA = 700;
  setTimeout(function () {
    mostrarMovimientos()
  }, TIEMPO_DE_ESPERA)

})
