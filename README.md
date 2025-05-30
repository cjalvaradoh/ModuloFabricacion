# Módulo de Fabricación

El Módulo de Fabricación es una solución desarrollada para gestionar y dar seguimiento a los procesos de producción dentro de la empresa. Su objetivo es centralizar la información de fabricación, mejorar la trazabilidad y optimizar la operación diaria del área productiva.

---

## Funcionalidades principales

- **Gestión de órdenes de producción**: creación, seguimiento y cierre de órdenes.
- **Control de procesos de fabricación**: registro de etapas, tiempos y responsables.
- **Asignación de operarios y máquinas**: distribución eficiente de recursos.
- **Gestión de materiales**: control de insumos utilizados por producto.
- **Reportes e indicadores**: generación de informes sobre productividad y eficiencia.
- **Integración con módulos existentes**: compatibilidad con los sistemas actuales de gestión empresarial.

---

## Prototipo

Aquí se muestra un esquema básico del diseño y flujo del módulo, incluyendo las pantallas principales para la gestión de órdenes, seguimiento de procesos y visualización de reportes.

### Inicio de Sesión
![image](https://github.com/user-attachments/assets/769a34d1-bd34-405d-bed4-cb0bc39dc27d)

### Registro
![image](https://github.com/user-attachments/assets/2f2baaa1-38cb-40d6-9512-529029efb5d7)

### Dashboard
![image](https://github.com/user-attachments/assets/4c91b463-3f15-47c8-a32d-14708655b009)

### Orden Producción
![image](https://github.com/user-attachments/assets/1b09f442-3306-4356-afbe-5e050d857e04)

### Seguimiento Producción
![image](https://github.com/user-attachments/assets/1f5e8fa4-efb6-4a72-83be-a5699d3c0c5b)

### Materiales
![image](https://github.com/user-attachments/assets/3e387aba-2dae-49af-a153-6f4b9906ddc1)

### Reportes
![image](https://github.com/user-attachments/assets/cbe8900d-6d80-4d3f-bb4e-9d5a5bf89c2d)

---

## Instalación

Sigue los siguientes pasos para instalar e integrar el módulo:

1. **Clona este repositorio:**
   ```bash
   git clone https://github.com/usuario/modulo-fabricacion.git

2. **Abre el proyecto en tu entorno de desarrollo.**
3. **Configura la conexión a la base de datos en el archivo appsettings.json (o el archivo de configuración correspondiente).**
4. **Ejecuta las migraciones (si aplica):**
    ```bash
   dotnet ef database update
5. **Ejecuta la aplicación:**
    ```bash
   dotnet run
    
## Requisitos previos
- .NET SDK
- SQL Server
- Docker
- Visual Studio o Visual Studio Code
- Herramientas EF Core
  
