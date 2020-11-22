using System.Web;
using System.Web.Optimization;

namespace SureNkap
{
    public class BundleConfig
    {
        // Pour plus d'informations sur le regroupement, visitez https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilisez la version de développement de Modernizr pour le développement et l'apprentissage. Puis, une fois
            // prêt pour la production, utilisez l'outil de génération à l'adresse https://modernizr.com pour sélectionner uniquement les tests dont vous avez besoin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/assets/vendor_components/jquery-3.3.1/jquery-3.3.1.js",
                      "~/assets/vendor_components/jquery-ui/jquery-ui.js",
                      "~/assets/vendor_components/popper/dist/popper.min.js",
                      "~/assets/vendor_components/bootstrap/dist/js/bootstrap.js",
                      "~/assets/vendor_components/jquery-slimscroll/jquery.slimscroll.js",
                      "~/assets/vendor_components/fastclick/lib/fastclick.js",
                      "~/assets/vendor_components/sweetalert/sweetalert.min.js",
                      "~/assets/vendor_components/sweetalert/jquery.sweet-alert.custom.js",
                      "~/scripts/js/template.js",
                      "~/Scripts/js/pages/validation.js"));

          

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/assets/vendor_components/bootstrap/dist/css/bootstrap.min.css",
                "~/assets/vendor_components/font-awesome/css/font-awesome.min.css",
                "~/Content/css/bootstrap-extend.css",            
                "~/Content/css/master_style.css",
                "~/Content/css/skins/_all-skins.css",
                "~/assets/vendor_components/sweetalert/sweetalert.css"));

            bundles.Add(new StyleBundle("~/Content/login").Include(
               "~/assets/vendor_components/bootstrap/dist/css/bootstrap.min.css",
               "~/Content/css/bootstrap-extend.css",
               "~/Content/css/master_style.css",
               "~/Content/css/skins/_all-skins.css"));

            bundles.Add(new ScriptBundle("~/bundles/login").Include(
               "~/assets/vendor_components/jquery-3.3.1/jquery-3.3.1.js",
                "~/assets/vendor_components/popper/dist/popper.min.js",
                "~/assets/vendor_components/bootstrap/dist/css/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                "~/assets/vendor_components/jquery-datatable/dataTables.bootstrap4.min.css",
                "~/assets/vendor_components/jquery-datatable/fixedeader/dataTables.fixedcolumns.bootstrap4.min.css",
                "~/assets/vendor_components/jquery-datatable/fixedeader/dataTables.fixedheader.bootstrap4.min.css",
                "~/Content/css/editor.dataTables.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/js/bundles/datatablescripts.bundle.js",
                "~/assets/vendor_components/jquery-datatable/buttons/dataTables.buttons.min.js",
                "~/assets/vendor_components/jquery-datatable/buttons/buttons.bootstrap4.min.js",
                "~/assets/vendor_components/jquery-datatable/buttons/buttons.colVis.min.js",
                "~/assets/vendor_components/jquery-datatable/buttons/buttons.html5.min.js",
                "~/assets/vendor_components/jquery-datatable/buttons/buttons.print.min.js",
                "~/Scripts/js/pages/tables/jquery-datatable.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/editable").Include(
                "~/assets/vendor_components/editable-table/mindmup-editabletable.js",
                "~/Scripts/js/pages/tables/editable-table.js"));
        }
    }
}
