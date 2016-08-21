/* TODO:
- minify files -> https://github.com/terinjokes/gulp-uglify?
- adpapt sass code
- launch task in visual studio
*/
'use strict';

var gulp = require('gulp');
var sass = require('gulp-sass');
var ts = require('gulp-typescript');
var gettext = require('gulp-angular-gettext');

gulp.task('default', function() {
  // place code for your default task here
});

/* SASS */
gulp.task('sass', function () {
	return gulp.src('./Content/**/*.scss')
		.pipe(sass().on('error', sass.logError))
		.pipe(gulp.dest('./Content/'));
});

gulp.task('sass:watch', function () {
	gulp.watch('./Content/**/*.scss', ['sass']);
});

/* TYPESCRIPT */
var tsProject = ts.createProject({
	target: 'ES5',
	module : 'amd',
	allowJs : true,
	declaration: false,
	noExternalResolve: false,
	//sortOutput: true
});

gulp.task('typescript', function () {
	return gulp.src(['./Angular/**/*.ts'])
		.pipe(ts(tsProject))
		.pipe(gulp.dest('./Angular/'));
});

gulp.task('typescript:watch', function () {
	gulp.watch('./Angular/**/*.ts', ['typescript']);
});

/* GET TEXT */
gulp.task('pot', function () {
    return gulp.src(['./Angular/**/*.html', './Angular/**/*.js'])
        .pipe(gettext.extract('template.pot', {
            // options to pass to angular-gettext-tools...
        }))
        .pipe(gulp.dest('./Angular/po/'));
});

gulp.task('translations', function () {
    return gulp.src('./Angular/po/**/*.po')
        .pipe(gettext.compile({
            // options to pass to angular-gettext-tools...
            format: 'json'
        }))
        .pipe(gulp.dest('./Angular/translations/'));
});


/* GLOBAL TASKS */
gulp.task('build', ['sass', 'typescript', 'pot']);
gulp.task('watch', ['sass:watch', 'typescript:watch']);