CREATE DATABASE inventory;
GO 
USE inventory;

CREATE TABLE movimiento(
idCodigo INT IDENTITY PRIMARY KEY,
codigoProducto VARCHAR (150),
fechaVencimiento DATETIME,
fechaIngreso DATETIME,
fechaSalida DATETIME,
creacionRegistro DATETIME,
cantidad INT,
tipoMovimiento INT,
usuarioId int
);

CREATE TABLE producto(
codigoProducto VARCHAR (150) PRIMARY KEY,
nombreProducto VARCHAR (150),
cantidad INT,
categoriaId VARCHAR (150)

);

CREATE TABLE categoria(
categoriaId VARCHAR (150) PRIMARY KEY,
nombreCategoria VARCHAR (150)
);

CREATE TABLE usuario(
usuarioId INT PRIMARY KEY,
nombre VARCHAR (150),
apellido1 VARCHAR (150),
apellido2 VARCHAR (150),
correoElectronico VARCHAR (150),
nombreUsuario VARCHAR (150),
contrasennaUsuario VARCHAR (150),
rol_Id INT
)


CREATE TABLE ingresoUsuario( 
ingresosId INT IDENTITY PRIMARY KEY,
usuarioId INT,
fecha DATETIME
)

CREATE TABLE rol(
rol_Id INT IDENTITY PRIMARY KEY,
rolNombre VARCHAR (150)
)

ALTER TABLE movimiento 
ADD CONSTRAINT FK_Movimeinto_Usuario
FOREIGN KEY (usuarioId) REFERENCES usuario(usuarioId);

ALTER TABLE movimiento 
ADD CONSTRAINT FK_Movimiento_Producto
FOREIGN KEY (codigoProducto) REFERENCES producto(codigoProducto);

ALTER TABLE ingresoUsuario 
ADD CONSTRAINT FK_IngresoUsuario_Usuario
FOREIGN KEY (usuarioId) REFERENCES usuario(usuarioId);

ALTER TABLE usuario 
ADD CONSTRAINT FK_Usuario_Rol
FOREIGN KEY (rol_Id) REFERENCES rol(rol_Id);

ALTER TABLE producto 
ADD CONSTRAINT FK_Categoria_Producto
FOREIGN KEY (categoriaId) REFERENCES categoria(categoriaId);

INSERT INTO categoria (categoriaId, nombreCategoria) VALUES
(1, 'Bebidas'),
(2, 'Congelados'),
(3, 'Ferretería'),
(4, 'Farmacia'),
(5, 'Carne'),
(6, 'Comestibles'),
(7, 'Juguetería'),
(8, 'Enlatados');
