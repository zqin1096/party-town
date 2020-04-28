const { buildLogger } = require('jege/server');
const cheerio = require('cheerio');
const del = require('del');
const fs = require('fs');
const gulp = require('gulp');
const path = require('path');

const buildLog = buildLogger('[trojan-blitz]');

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

  const scriptHtml = `
<script>
  window.onload = function handleOnLoad() {
    if (window.screen.availWidth <= 480) {
      const body = document.getElementsByTagName('body')[0]
      body.classList.add('mobile');
    }
  }
  window.onresize = function handleOnResize() {
    const body = document.getElementsByTagName('body')[0]
    if (window.screen.availWidth <= 480) {
      const body = document.getElementsByTagName('body')[0]
      body.classList.add('mobile');
    } else {
      body.classList.remove('mobile');
    }
  }
</script>
`;
  const styleHtml = `
<style>
  html, body {
    margin: 0;
    padding: 0;
  }
  body.mobile {
    font-size: 5vw;
  }
  #wrap {
    margin: 80px auto 0;
    width: 960px;
  }
  @media (max-width: 480px) {
    body {
      font-size: 15vw;
    }
  }
  .h2 {
    margin-top: 0.5em;
    margin-bottom: 0.2em;
  }
  .warn {
    color: #767676;
  }
  .webgl-content {
    position: static;
    transform: none;
  }
  #desc {
    margin-top: 2.6em;
  }
  .ref {
    margin-top: 0.6em;
  }
</style>
`;

  const warnHtml = `
<p class="warn">
  This application is designed to run in devices that support WebGL, with the viewport ratio ideally 2:1. Position your device horizontally so that width exceeds height.
  Note also that the game is served on a Photon public cloud. This may no longer be available in the future.
</p>
`;

  const descHtml = `
<div id="desc">
  <h2 class="h2">
    Trojan Blitz
  </h2>
  <p>
    A highly versatile, fast-paced, deck-building card game for the mobile platform. Intuitive play with deck building strategy to compete in battles on the clock. Players choose their character at the start of a match from a randomly generated selection, and then play using the deck they have customized with their own cards.
  </p>
  <h2 class="h2">
    Gameplay
  </h2>
  <div style="width:100%;height:0px;position:relative;padding-bottom:48.387%;"><iframe src="https://streamable.com/e/8qizpb" frameborder="0" width="100%" height="100%" allowfullscreen style="width:100%;height:100%;position:absolute;left:0px;top:0px;overflow:hidden;"></iframe></div>
  <h2 class="h2">
    Multiplayer Game - System Design
  </h2>
  <p>
    Trojan Blitz, created with Unity, written in C#, is a multiplayer game application, that is designed to run in mobile devices. Using remote procedure calls (RPCs), it propagates a state of the application to player nodes in the network. It might as well have been architected in such a way that it only broadcasts "action" to the participating nodes. Other than multi-agent element, Trojan Blitz has an architecture typically expected of game applications. GameManager, Character, Card, Player are fundamental classes that make up this game.
  </p>
  <p class="ref">
    <a href="https://github.com/zqin1096/party-town">Github</a>
  </p>
  <p class="h2">
    This project is initiated as the course assignment of CSCI599 - Special Topics (Social Mobile Games), at the University of Southern California in Spring 2020.
  </p>
</div>
`;

  $('head').append(styleHtml);
  $('head').append(scriptHtml);
  $('body').prepend('<div id="wrap"></div>');
  const webglContent = $('.webgl-content');
  webglContent.append(descHtml);

  $('#wrap').prepend(warnHtml);
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
