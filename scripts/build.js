const { buildLogger } = require('jege/server');
const cheerio = require('cheerio');
const del = require('del');
const fs = require('fs');
const gulp = require('gulp');
const path = require('path');

const buildLog = buildLogger('trojan-blitz');

const paths = {
  docs: path.resolve(__dirname, '../docs'),
  webgl: path.resolve(__dirname, '../webgl'),
};

gulp.task('clean', () => {
  const cleanPaths = [
    `${paths.docs}/**/*`,
  ];

  buildLog('clean', 'cleanPaths: %j', cleanPaths);

  return del(cleanPaths, {
    force: true,
  });
});

gulp.task('copy-webgl', () => {
  const srcPath = `${paths.webgl}/**/*`;
  const destPath = `${paths.docs}`;
  buildLog('copy-webgl', 'srcPath: %s, destPath: %s', srcPath, destPath);

  return gulp.src(srcPath)
    .pipe(gulp.dest(destPath));
});

gulp.task('copy-index-html', () => {
  const srcPath = `${paths.docs}/index.html`;
  const destPath = `${paths.docs}/index_original.html`;
  buildLog('copy-index-html', 'srcPath: %s, destPath: %s', srcPath, destPath);
  fs.copyFileSync(srcPath, destPath);

  return Promise.resolve();
});

gulp.task('modify-html', () => {
  const srcPath = path.resolve(paths.docs, 'index.html');
  buildLog('modify-html', 'srcPath: %s', srcPath);

  const indexHtml = fs.readFileSync(srcPath).toString();
  const $ = cheerio.load(indexHtml);

  const styleHtml = `
<style>
  .webgl-content {
    position: static;
    transform: none;
  }
</style>
`;

  const descHtml = `
<div id="desc">
  <h2>
    123
  </h2>
  <p>
    power
  </p>
</div>
`;
  $('body').prepend('<div id="wrap"></div>');
  $('head').append(styleHtml);

  const webglContent = $('.webgl-content');
  webglContent.append(descHtml);

  $('#wrap').append(webglContent);
  const nextHtml = $.html();

  fs.writeFileSync(srcPath, nextHtml, {
    flag: 'w',
  });
  return Promise.resolve();
});

gulp.task('build', gulp.series('clean', 'copy-webgl', 'copy-index-html', 'modify-html'));

if (require.main === module) {
  const buildTask = gulp.task('build');
  buildTask();
}
