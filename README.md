# TourDeFrance

|             |Build Status|
|-------------|:----------:|
|**Linux**    |[![Build Status](https://travis-ci.org/pierresebastien/TourDeFrance.svg)](https://travis-ci.org/pierresebastien/TourDeFrance)|
|**Windows**  |/|

##Roadmap

* Add authentication in API (use [identity server](https://identityserver.io/) ?)
* Add [SignalR](https://github.com/SignalR/SignalR) to push event to client browsers
* Add a lot of unit tests and migrate to NUnit3
* Use [travis](https://travis-ci.org/) & [AppVeyor](https://www.appveyor.com/) for continious integration
* Use [swagger](http://swagger.io/swagger-ui/) to document the api (check usage of [nancy.openapi](https://github.com/thesheps/nancy.openapi) or [nancy.swagger](https://github.com/yahehe/Nancy.Swagger))
* Add support for dot net core (check compatibility of nuget dependencies)
* Create project with [wixsharp](https://wixsharp.codeplex.com/) to generate windows setup with [wix toolset](http://wixtoolset.org/)
* Add script to build linux (debian) package
* Add i18n support in API & core project (check [NGettext](https://github.com/neris/NGettext) or [i18n-Complete](https://github.com/dotnetwise/i18N-Complete))
* Use code coverage (use [coveralls](https://coveralls.io/))
* Use [coverity](https://scan.coverity.com) ?
* Review and improve gulp tasks
* Check the integration of some ui libraries :
	+ [Angular material data table](https://github.com/daniel-nagy/md-data-table)
	+ [Angular file manager](https://github.com/joni2back/angular-filemanager)
	+ [Angular image cropper](https://github.com/bcabanes/angular-image-cropper)
	+ [Angular xeditable](https://vitalets.github.io/angular-xeditable/) (check if integrated in angular material)
	+ [Angular surprise](https://github.com/rafaelcamargo/ng-surprise)
	+ [Angular color picker](https://github.com/brianpkelley/md-color-picker)
	+ [Angular drag & drop](https://github.com/kshutkin/drag_n_drop)
	+ [Angular confirm button](https://github.com/MrBoolean/ng-confirm)
	+ [Angular popover](http://verical.github.io/#/ngDropover) (check if integrated in angular material)
	+ [Angular loading](http://bsalex.github.io/angular-loading-overlay/_site/) to replace angular spinner
	+ [Angular incremental list](https://github.com/tfoxy/angular-incremental-list)
	+ Graphs with [Angular nvd3](http://krispo.github.io/angular-nvd3/#/) or [Angular chart.js](http://jtblin.github.io/angular-chart.js/)
* Creation of new ui project with [react](https://facebook.github.io/react/) and [bootstrap](http://getbootstrap.com/), [semantic ui](http://semantic-ui.com/) or [metro ui](https://metroui.org.ua/)
* Migrate current ui project to Angular2 ?
* Review cache system, drop redis support ? use [Akavache](https://github.com/akavache/Akavache) instead ?
* Check usage of [RethinkDb](https://www.rethinkdb.com/)
	
## Node.js

This project use some Node.js dependencies, you can downlaod and install it [here](https://nodejs.org/en/download/).

Here is the list of package used by this project :

* bower
* typescript
* typings
* node-sass
* gulp

You can install them with the following command : 

```
npm install {package} -g
```

Once these packages are installed, go to the ui project and execute the following commands :

```
npm install
bower install
typings install
```