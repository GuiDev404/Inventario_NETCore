if(jQuery.validator){
  jQuery.validator.addMethod("sinSimbolos", function(inputValue, element, validar) {

    if(validar){
      // permite solo letras y numeros
      const regex = new RegExp(/^[A-Z\s0-9]+$/i)
  
      return typeof(inputValue) === 'string' && regex.test(inputValue)
    }
   
    return true;
  });
  
  jQuery.validator.addMethod("soloLetras", function(inputValue, element, validar) {

    if(validar){
      // permite solo letras
      const regex = new RegExp(/^[A-Z\s]+$/i)
  
      return typeof(inputValue) === 'string' && regex.test(inputValue)
    }
   
    return true;
  });
  
  jQuery.validator.addMethod("imgValida", function(_value, element, validar) {
    if(element.files[0]){
      const tipoDeArchivo = element.files[0].type;
      
      console.log(tipoDeArchivo?.includes('image/'));
      
      if(validar && tipoDeArchivo){
        return tipoDeArchivo?.includes('image/')
      }
    }
  
    return true;
  });
  
  jQuery.validator.addMethod("decimales", function(inputValue, element, validar) {
    if(validar){
      const regex = new RegExp(/^[0-9]+([,][0-9]+)?$/)
  
      return typeof(inputValue) === 'string' && regex.test(inputValue)
    }
    
  });
  
  jQuery.validator.addMethod("dniValido", function(inputValue, element, validar) {
    if(validar){
      const dniRegex = new RegExp(/^[0-9]{7,8}$/)
  
      return dniRegex.test(inputValue) && parseInt(inputValue) > 0;
    }
    
  });
  
  jQuery.validator.addMethod("seleccionRequerida", function(inputValue, element, requerida) {
    
    if(requerida){
      return parseInt(inputValue) > 0 && !element.disabled;
    }
  
    return true;
  });
  
  jQuery.validator.addMethod("cpValido", function(inputValue, element, validar) {
    if(validar && inputValue.trim()){
      const codigoPostalRegex = new RegExp(/^[0-9]{4}$/)
  
      return codigoPostalRegex.test(inputValue);
    }
  
    return true;
  });
  
  jQuery.validator.addMethod("telefonoValido", function(inputValue, element, validar) {
    if(validar && inputValue.trim()){
      // const telefonoPatron = new RegExp(/^(\+54\s)?(\d{4}\s)?(15\s)?\d{2}-\d{4}$/);
      const telefonoPatron = new RegExp(/^\+54 \d{4} \d{2}-\d{4}$/);
  
      return telefonoPatron.test(inputValue.trim());
    }
  
    return true;
  });
}

const ESTADOS_MOVIMIENTOS = {
  ENTRADA: 0,
  SALIDA: 1,
}

function filtrarPorBusqueda(e){
  const busqueda = typeof e === 'string' ? e : e.target.value.trim().toUpperCase();
  const filas = [...document.querySelectorAll('#tBody tr')]
  const mensajeFila = document.querySelector('#tFooter tr td:first-child');
 
  filas.forEach(fila=> {
    fila.classList.add('d-none')
    mensajeFila.classList.add('d-none');
    
    // CELDAS DONDE SE HARA LA BUSQUEDA (TODAS LAS CELDAS MENOS LA DE LOS BOTONES)
    const tds = fila.querySelectorAll('td:not(:last-child)')
    
    // AL MENOS UNA CELDA(<td>) COINCIDE CON LA BUSQUEDA
    const busquedaOk = [...tds].some(td=> td.innerText.trim().toUpperCase().includes(busqueda))
    if(busquedaOk){
      fila.classList.remove('d-none')
    }

    // NINGUNA FILA(<tr>) TIENE COINCIDENCIA
    const noHayResultados = filas.every(tr=> tr.classList.contains('d-none'))

    if(noHayResultados){
      mensajeFila.innerHTML = noHayResultados 
        ? `No hay resultados para <strong> ${busqueda} </strong>` 
        : ''
      
      mensajeFila.classList.remove('d-none');
    }  
    

  })

}

function toast({ type, message, title, fnClose }) {
  Swal.fire({
    title: title,
    text: message,
    icon: type,
    toast: true,
    position: "bottom-end",
    timer: 2000,
    timerProgressBar: true,
    showConfirmButton: false,
    customClass: {
      container: 'sw-toast-container'
    },
    didClose: function() {
      fnClose && fnClose();
    },
    didOpen: (toast) => {
      toast.addEventListener('mouseenter', Swal.stopTimer)
      toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
  });
}

function nullFallback(valor, texto = ''){
  return valor === null ? texto : valor; 
}

function volver() {
  return history.go(-1);
}

// function diasEntreFechas(fechaInicio, fechaFin) {
//   const fechaInicioMs = fechaInicio.getTime();
//   const fechaFinMs = fechaFin.getTime();

//   if(!fechaFinMs || !fechaInicioMs || fechaFinMs < fechaInicioMs) return '';
//   const diff = Math.abs(fechaFinMs - fechaInicioMs);

//   // (1000*60*60*24) --> milisegundos -> segundos -> minutos -> horas(24) = 1 dia
//   const DIA_EN_MS = (1000*60*60*24) 
//   return Math.floor(diff / DIA_EN_MS);
// }

const fechaAInput = (fecha)=> {
  const año = fecha.getFullYear()
  let mes = fecha.getMonth() + 1;
  let diaDelMes = fecha.getDate()

  if(mes < 10){
    mes = '0'+mes
  } 

  if(diaDelMes < 10){
    diaDelMes = '0'+diaDelMes
  } 

  return `${año}-${mes}-${diaDelMes}`
};

// const sumarDiasAFecha = (fecha, dias = 1)=> {
//   const fechaMasDias = new Date(fecha.setDate(fecha.getDate() + dias))
//   const fechaInput = fechaAInput(fechaMasDias);

//   return fechaInput
// }

// const restarDiasAFecha = (fecha, dias = 1)=> {
//   const fechaMenosDias = new Date(fecha.setDate(fecha.getDate() - dias))
//   const fechaInput = fechaAInput(fechaMenosDias);
  
//   return fechaInput
// }

const maxLengthInput = (selector, max) => {
  const element = document.querySelector(selector);

  element && element.addEventListener('input', (e)=> {
    if(e.target.value.length >= max){
      e.target.value = e.target.value.slice(0, max);
    }
  })
}

function syncBackendError (error) {
  const input = document.getElementById(error.field);
  const errorElement = document.createElement('p')
  errorElement.id = `${error.field}-error`
  errorElement.className = 'text-danger small d-block mt-1'
  errorElement.innerText = error.messages[0]

  input.insertAdjacentElement('afterend', errorElement)

  setTimeout(() => {
    errorElement.remove();
  }, 3000);
}