let html5QrCode = null;
const imgReferencial = document.getElementById("imagenReferencial")

function leerCodigoDeBarra (inputId){
  $("#tempInputId").val(inputId);
  $('#modalCodigoBarra').modal('show')
}

function lecturaCorrecta(codigoTexto, codigoObjeto) {
  // handle the scanned code as you like, for example:
  console.log(`Code matched = ${codigoTexto}`, codigoObjeto);

  detenerCamara()

  const inputId = $("#tempInputId").val()
  $(inputId).val(codigoTexto)
  $('#modalCodigoBarra').modal('hide')
  $("#tempInputId").val("")
}

function errorLectura(error) {
  // handle scan failure, usually better to ignore and keep scanning.
  // for example:
  //console.warn(`Code scan error = ${error}`);
}

function camaraSeleccionada (elemento) {

  let idCamaraSeleccionada = elemento.value;
  console.log('idCamaraSeleccionada: ', idCamaraSeleccionada);

  if (idCamaraSeleccionada){
    imgReferencial.classList.add('d-none');
    imgReferencial.classList.remove('d-block');
  
    html5QrCode = new Html5Qrcode("reader");
    html5QrCode.start(
      idCamaraSeleccionada, 
      {
        fps: 10,    // Optional, frame per seconds for qr code scanning
        qrbox: { width: 250, height: 250 }  // Optional, if you want bounded box UI
      },
      lecturaCorrecta,
      errorLectura
    )
    .catch((err) => {
      // Start failed, handle it.
    });
  }


}

function detenerCamara () {

  html5QrCode
    .stop()
    .then((ignore) => {
      // QR Code scanning is stopped.
      imgReferencial.classList.remove('d-none');
      imgReferencial.classList.add('d-block');
      // document.getElementById("listaCamaras").value = "";
    }).catch((err) => {
      // Stop failed, handle it.
    });

}

/* ============ PARA IMAGENES ============ */

const html5QrCode2 = new Html5Qrcode("reader-file");
// File based scanning
const fileinput = document.getElementById('qr-input-file');
fileinput.addEventListener('change', e => {
  if (e.target.files.length == 0) {
    // No file selected, ignore 
    return;
  }

  const imageFile = e.target.files[0];
  // Scan QR Code
  html5QrCode2.scanFile(imageFile, true)
  .then(lecturaCorrecta)
  .catch(err => {
    // failure, handle it.
    console.log(`Error scanning file. Reason: ${err}`)
  });
});

window.addEventListener('DOMContentLoaded', function (){
  /* ============ GET CAMARAS ============ */
  // This method will trigger user permissions
  
  Html5Qrcode.getCameras()
    .then(camaras => {

      if (camaras && camaras.length) {
        console.log(camaras);
        // let camaraId = camaras[0].id;
        let select = document.getElementById("listaCamaras");
        let html = `<option value="" selected>Seleccione una camara</option>`;

        camaras.forEach(camara => {
          html += `<option value="${camara.id}">${camara.label}</option>`
        });

        select.innerHTML = html;
      }
    }).catch(err => {
      // handle err
    });
})


const btn_search_by_barcode = document.getElementById('btn_search_by_barcode')
btn_search_by_barcode.addEventListener('click', function (){
  leerCodigoDeBarra("#buscador")

  // $('#modalCodigoBarra').on('hide.bs.modal', searchByCode);
  const modalCodigoBarra = document.getElementById('modalCodigoBarra')

  function searchByCode() {
    const valorBusqueda = $('#buscador').val()
    filtrarPorBusqueda(valorBusqueda)
    console.log('SE CERRO JIJO');

    modalCodigoBarra.removeEventListener('hide.bs.modal', searchByCode)
  }
  
  modalCodigoBarra.addEventListener('hide.bs.modal', searchByCode)
})