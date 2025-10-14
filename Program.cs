using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InventarioLINQ
{
    class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string rutaArchivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "productos.csv");
            List<Producto> productos = CargarProductos(rutaArchivo);

            if (productos.Count == 0)
            {
                Console.WriteLine("⚠ No se encontraron productos. Verifica 'productos.csv'.");
                return;
            }

            // CONSULTAS
            var bajoStock = productos.Where(p => p.Stock < 10).ToList();
            var ordenadosPrecio = productos.OrderByDescending(p => p.Precio).ToList();
            double totalInventario = productos.Sum(p => p.Precio * p.Stock);
            var agrupados = productos.GroupBy(p => p.Categoria);

            // RESULTADOS EN CONSOLA
            Console.WriteLine("=== Inventario Cargado ===\n");
            Console.WriteLine("Productos con stock menor a 10:");
            bajoStock.ForEach(p => Console.WriteLine($"{p.Nombre} - Stock: {p.Stock}"));
            Console.WriteLine("\nProductos ordenados por precio:");
            ordenadosPrecio.ForEach(p => Console.WriteLine($"{p.Nombre} - Q{p.Precio}"));
            Console.WriteLine($"\nValor total del inventario: Q{totalInventario:F2}\n");

            Console.WriteLine("Productos agrupados por categoría:");
            foreach (var grupo in agrupados)
            {
                Console.WriteLine($"\nCategoría: {grupo.Key}");
                foreach (var p in grupo)
                    Console.WriteLine($"  - {p.Nombre} (Q{p.Precio}, Stock {p.Stock})");
            }

            // EXPORTAR RESULTADOS
            ExportarResultados("resultado.txt", bajoStock, ordenadosPrecio, totalInventario, agrupados);
            Console.WriteLine("\ns Resultados exportados a 'resultado.txt'");
        }

        static List<Producto> CargarProductos(string ruta)
        {
            var productos = new List<Producto>();
            if (!File.Exists(ruta)) return productos;

            var lineas = File.ReadAllLines(ruta).Skip(1);
            foreach (var linea in lineas)
            {
                var partes = linea.Split(',');
                if (partes.Length < 5) continue;
                productos.Add(new Producto
                {
                    Id = int.Parse(partes[0]),
                    Nombre = partes[1],
                    Categoria = partes[2],
                    Precio = double.Parse(partes[3]),
                    Stock = int.Parse(partes[4])
                });
            }
            return productos;
        }

        static void ExportarResultados(
            string archivo,
            List<Producto> bajoStock,
            List<Producto> ordenados,
            double total,
            IEnumerable<IGrouping<string, Producto>> agrupados)
        {
            using (StreamWriter sw = new StreamWriter(archivo))
            {
                sw.WriteLine("=== RESULTADOS DEL INVENTARIO ===\n");
                sw.WriteLine("1️⃣ Productos con stock menor a 10:");
                foreach (var p in bajoStock)
                    sw.WriteLine($"{p.Nombre} - Stock: {p.Stock}");
                sw.WriteLine("\n2️⃣ Productos ordenados por precio:");
                foreach (var p in ordenados)
                    sw.WriteLine($"{p.Nombre} - Q{p.Precio}");
                sw.WriteLine($"\n3️⃣ Valor total del inventario: Q{total:F2}\n");
                sw.WriteLine("4️⃣ Agrupados por categoría:");
                foreach (var grupo in agrupados)
                {
                    sw.WriteLine($"\nCategoría: {grupo.Key}");
                    foreach (var p in grupo)
                        sw.WriteLine($"  - {p.Nombre} (Stock {p.Stock}, Q{p.Precio})");
                }
            }
        }
    }
}
