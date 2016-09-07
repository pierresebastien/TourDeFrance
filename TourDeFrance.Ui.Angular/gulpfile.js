/* TODO:
- minify files -> https://github.com/terinjokes/gulp-uglify?
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
	return gulp.src('./content/sass/**/*.scss')
		.pipe(sass().on('error', sass.logError))
		.pipe(gulp.dest('./content/css/'));
});

gulp.task('sass:watch', function () {
	gulp.watch('./content/sass/**/*.scss', ['sass']);
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
	gulp.src(['./**/*.ts', '!node_modules/**/*.ts'])
		.pipe(ts(tsProject))
		.pipe(gulp.dest('./'));
});

gulp.task('typescript:watch', function () {
	gulp.watch(['./app/**/*.ts','./models/**/*.ts','./services/**/*.ts'], ['typescript']);
});

/* GET TEXT */
gulp.task('pot', function () {
    return gulp.src(['./app/**/*.html', './app/**/*.js'])
        .pipe(gettext.extract('template.pot', {
            // options to pass to angular-gettext-tools...
        }))
        .pipe(gulp.dest('./po/'));
});

gulp.task('translations', function () {
    return gulp.src('./po/**/*.po')
        .pipe(gettext.compile({
            // options to pass to angular-gettext-tools...
            format: 'json'
        }))
        .pipe(gulp.dest('./translations/'));
});


/* GLOBAL TASKS */
gulp.task('build', ['sass', 'typescript', 'pot']);
gulp.task('watch', ['sass:watch', 'typescript:watch']);