// Cascada País -> Estado/Provincia -> Ciudad
function initCascadaUbicacion(prefijo) {
    const selectPais = document.getElementById(`${prefijo}Pais`);
    const selectEstado = document.getElementById(`${prefijo}EstadoProvincia`);
    const selectCiudad = document.getElementById(`${prefijo}Ciudad`);

    if (!selectPais || !selectEstado || !selectCiudad) return;

    selectPais.addEventListener('change', async () => {
        selectEstado.innerHTML = '<option value="">Cargando...</option>';
        selectCiudad.innerHTML = '<option value="">Selecciona un estado primero</option>';

        const respuesta = await fetch(`/Localizacion/Estados?pais=${encodeURIComponent(selectPais.value)}`);
        const estados = await respuesta.json();

        selectEstado.innerHTML = '<option value="">Selecciona...</option>';
        estados.forEach(e => {
            const opt = document.createElement('option');
            opt.value = e;
            opt.textContent = e;
            selectEstado.appendChild(opt);
        });
    });

    selectEstado.addEventListener('change', async () => {
        selectCiudad.innerHTML = '<option value="">Cargando...</option>';

        const respuesta = await fetch(`/Localizacion/Ciudades?pais=${encodeURIComponent(selectPais.value)}&estado=${encodeURIComponent(selectEstado.value)}`);
        const ciudades = await respuesta.json();

        selectCiudad.innerHTML = '<option value="">Selecciona...</option>';
        ciudades.forEach(c => {
            const opt = document.createElement('option');
            opt.value = c;
            opt.textContent = c;
            selectCiudad.appendChild(opt);
        });
    });
}

// Ojito de mostrar/ocultar contraseña
function initTogglePassword() {
    document.querySelectorAll('.aura-toggle-password').forEach(boton => {
        boton.addEventListener('click', () => {
            const input = document.getElementById(boton.dataset.target);
            const esVisible = input.type === 'text';
            input.type = esVisible ? 'password' : 'text';
            boton.textContent = esVisible ? '👁' : '🙈';
        });
    });
}

// Formato automático de Cédula: 000-0000000-0
function formatearCedula(valor) {
    const digitos = valor.replace(/\D/g, '').slice(0, 11);
    if (digitos.length <= 3) return digitos;
    if (digitos.length <= 10) return `${digitos.slice(0, 3)}-${digitos.slice(3)}`;
    return `${digitos.slice(0, 3)}-${digitos.slice(3, 10)}-${digitos.slice(10)}`;
}

// Formato automático de Teléfono: 000-000-0000
function formatearTelefono(valor) {
    const digitos = valor.replace(/\D/g, '').slice(0, 10);
    if (digitos.length <= 3) return digitos;
    if (digitos.length <= 6) return `${digitos.slice(0, 3)}-${digitos.slice(3)}`;
    return `${digitos.slice(0, 3)}-${digitos.slice(3, 6)}-${digitos.slice(6)}`;
}

function initFormatoAutomatico() {
    document.querySelectorAll('[data-formato="cedula"]').forEach(input => {
        input.setAttribute('maxlength', '13');
        input.setAttribute('inputmode', 'numeric');
        input.addEventListener('input', () => {
            input.value = formatearCedula(input.value);
        });
    });

    document.querySelectorAll('[data-formato="telefono"]').forEach(input => {
        input.setAttribute('maxlength', '12');
        input.setAttribute('inputmode', 'numeric');
        input.addEventListener('input', () => {
            input.value = formatearTelefono(input.value);
        });
    });
}

// Bloquea números y símbolos en campos de nombre — permite letras, espacios y tildes/ñ
function formatearSoloLetras(valor) {
    return valor.replace(/[^A-Za-zÁÉÍÓÚáéíóúÑñÜü\s]/g, '');
}

function initSoloLetras() {
    document.querySelectorAll('[data-formato="nombre"]').forEach(input => {
        input.addEventListener('input', () => {
            const posicion = input.selectionStart;
            const valorAnterior = input.value;
            input.value = formatearSoloLetras(input.value);
            if (input.value.length !== valorAnterior.length) {
                input.setSelectionRange(posicion - 1, posicion - 1);
            }
        });
    });
}

document.addEventListener('DOMContentLoaded', () => {
    initTogglePassword();
    initFormatoAutomatico();
    initSoloLetras();
});