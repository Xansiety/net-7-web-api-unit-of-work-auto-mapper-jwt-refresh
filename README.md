# Consultas de seguimiento

* Tracking o comportamiento de seguimiento
Controla si EF mantendra la informacion sobre una instancia entidad en su herramienta de seguimiento de cambios.
Si se hace este seguimiento, cualquier cambio detectado persistira hasta la base de datos cuando llamemos a la funcion `SaveChanges()`.
del contexto.


# Consultas de no seguimiento
Las consultas de no seguimiento(No Tracking) son útiles para operaciones de sólo lectura que no requieren que las entidades se mantengan en el contexto para su actualización o eliminación.
Su ejecución es mas rápida.
También proporcionara los resultados en función de los datos actuales en la base de datos, en lugar de los datos que se han modificado en el contexto pero no se han guardado en la base de datos.
Para habilitar el comportamiento de no seguimiento , podemos usar el método `AsNoTracking()`.

Otra forma de habilitarlo es establecer la propiedad `AsTracking` en `false` en el contexto.

```csharp
var context = new SchoolContext();
context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
```

