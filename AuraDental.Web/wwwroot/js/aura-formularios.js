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

document.addEventListener('DOMContentLoaded', () => {
    initTogglePassword();
});