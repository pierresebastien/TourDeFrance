/* TODO: 
- use pump ? see https://github.com/terinjokes/gulp-uglify?
- remove run-sequence if update to gulp 4
*/
'use strict';

var gulp = require('gulp');
var sass = require('gulp-sass');
var ts = require('gulp-typescript');
var concat = require('gulp-concat');
var rename = require("gulp-rename");
var uglify = require('gulp-uglify');
var gettext = require('gulp-angular-gettext');
var runSequence = require('run-sequence');
var clean = require('gulp-clean');

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

/* CONCAT */
gulp.task('concat:models', function() {
  return gulp.src(['models/enums.js', './models/**/!(enums)*.js'])
    .pipe(concat('models.js'))
    .pipe(gulp.dest('./dist/'));
});

gulp.task('concat:components', function() {
  return gulp.src(['./components/**/!(components)*.js', 'components/components.js'])
    .pipe(concat('components.js'))
    .pipe(gulp.dest('./dist/'));
});

gulp.task('concat:services', function() {
  return gulp.src(['./services/**/!(services)*.js', 'services/services.js'])
    .pipe(concat('services.js'))
    .pipe(gulp.dest('./dist/'));
});

gulp.task('concat:controllers', function() {
  return gulp.src(['app/shared/base_controller.js', './app/*/**/!(base_controller)*.js'])
    .pipe(concat('controllers.js'))
    .pipe(gulp.dest('./dist/'));
});

gulp.task('concat:app', function() {
  return gulp.src(['./app/config.js', './app/app.js'])
    .pipe(concat('main.js'))
    .pipe(gulp.dest('./dist/'));
});

gulp.task('concat', ['concat:models', 'concat:components', 'concat:services', 'concat:controllers', 'concat:app']);

/* UGLIFY */
gulp.task('uglify', function () {
  return gulp.src(['./dist/*.js'])
	.pipe(rename({ suffix: '.min' }))
    .pipe(uglify({mangle:false}))
    .pipe(gulp.dest('./dist/'));
});

/* CLEAN */
gulp.task('clean', function () {
    return gulp.src(['./dist/', './translations/', './content/css/'], {read: false})
        .pipe(clean());
});

/* GLOBAL TASKS */
gulp.task('default', function() {
  // place code for your default task here
});
gulp.task('build', function(callback) {
  runSequence('clean', ['sass', 'typescript', 'pot'], 'concat', 'uglify', callback);
});
gulp.task('watch', ['sass:watch', 'typescript:watch']);