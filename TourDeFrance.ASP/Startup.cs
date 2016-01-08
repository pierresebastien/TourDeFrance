using Owin;
using TourDeFrance.ASP.Common.Tools;
using TourDeFrance.Core;

namespace TourDeFrance.ASP
{
	// TODO: update bower libs

	// TODO: libraries to check + good practices
	// https://github.com/mgechev/angularjs-style-guide/blob/master/README-fr-fr.md
	// https://www.airpair.com/angularjs/posts/ng-directive-componentization-composition

	// semantic ui : http://semantic-ui.com/
	// mtro ui : http://metroui.org.ua/

	// internationalization : https://angular-gettext.rocketeer.be/
	// easter egg : https://github.com/rafaelcamargo/ng-surprise
	// side menu : https://github.com/dbtek/angular-aside
	// tables : http://lorenzofox3.github.io/smart-table-website/
	// confirm button : https://github.com/MrBoolean/ng-confirm
	// spinner : http://bsalex.github.io/angular-loading-overlay/_site/
	// drag & drop : https://github.com/kshutkin/drag_n_drop
	// color picker : https://github.com/brianpkelley/md-color-picker
	// regex support : https://github.com/zerohouse/ng-regex
	// popover : http://verical.github.io/#/ngDropover
	// dynamic list : http://jsbin.com/dixefo/embed?html,output
	// improved inputs check : https://github.com/assisrafael/angular-input-masks
	// edit in place : http://vitalets.github.io/angular-xeditable/
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ApplicationConfig config = ApplicationConfig.FromFile();
			OwinHelper helper = new OwinHelper(app);
			helper.Inititialize(config);
			helper.RegisterAuthentication();
			helper.RegisterMvcApplication();
			helper.RegisterWebApiApplication();
		}
	}
}