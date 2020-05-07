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

  const GameContainer = {
    width: '90vw',
    height: '56.25vw',
    maxWidth: '585px',
    maxHeight: '406.25px',
  };

  const headHtml = `
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link href="https://fonts.googleapis.com/css2?family=Merriweather:wght@400;700;900&display=swap" rel="stylesheet">
`;

  const scriptHtml = `
<script>
</script>
`;
  const styleHtml = `
<style>
  html, body {
    margin: 0;
    padding: 0;
    font-family: Calibri, Helvetica, Arial, sans-serif;
    line-height: 1.5;
  }
  body {
    font-size: 16px;
  }
  a, a:visited {
    color: #0F79D0;
    text-decoration: none;
  }
  a:hover {
    color: #0F79D0;
    text-decoration: underline;
  }
  h1, h2 {
    font-family: 'Merriweather', serif;
  }
  h1 {
    font-size: 32px;
    margin-top: 1.1em;
    margin-bottom: 0.2em;
  }
  h2 {
    border-bottom: 1px solid #d9d9d9;
    font-size: 28px;
    margin-top: 1.1em;
    margin-bottom: 0.2em;
  }
  ul {
    list-style: none;
    padding: 0;
  }
  #wrap {
    align-items: center;
    display: flex;
    flex-direction: column;
  }
  @media (max-width: 480px) {
    body {
      font-size: 14px;
    }
  }
  .warn {
    color: #b22b77;
  }
  .webgl-content {
    align-items: center;
    background-color: #e8e8e8;
    display: flex;
    flex-direction: column;
    padding: 1.8em 0;
    position: static;
    transform: none;
    width: 100%;
  }
  .footer {
    max-width: ${GameContainer.maxWidth};
    width: ${GameContainer.width};
  }
  .footer .title {
    display: none;
  }
  #desc {
    margin: 10px 0 60px 0;
  }
  .content-area {
    max-width: 650px;
    padding: 12px;
  }
  .media-container {
    align-items: center;
    display: flex;
    flex-direction: column;
  }
  .media {
    height: 47vw;
    max-height: 267px;
    max-width: 480px;
    width: 85vw;
  }
</style>
`;

  const warnHtml = `
<p class="warn content-area">
  This application is designed to run in devices that support WebGL, with the viewport ratio ideally 1.6:1. Position your device horizontally if possible to best enjoy the game.
  Note also that the game is served from a Photon public cloud and this may no longer be available in the future.
</p>
`;

  const descHtml = `
<div id="desc" class="content-area">
  <h1>
    TrojanBlitz
  </h1>
  <p>
    A versatile, fast-paced, deck-building card game for the mobile platform. You will be immersed into an intuitive play with deck building strategy to compete in battles on the clock. Players choose their character at the start of a match from a randomly generated selection, and then play using the deck they have customized with their own cards.
  </p>
  <p>
    Github: <a href="https://github.com/zqin1096/party-town">https://github.com/zqin1096/party-town</a>
  </p>
  <h2>
    Gameplay
  </h2>
  <div class="media-container">
    <div class="media" style="position:relative;"><iframe src="https://streamable.com/e/2f6bbj" frameborder="0" width="100%" height="100%" allowfullscreen style="width:100%;height:100%;position:absolute;left:0px;top:0px;overflow:hidden;"></iframe></div>
    <p>Gameplay demo</p>
    <iframe class="media" src="https://www.youtube.com/embed/LiBGdPvmNH4" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
    <p>Making footage</p>
  </div>
  <h2>
    Multiplayer Game - Engineering Aspects
  </h2>
  <p>
    TrojanBlitz, created with Unity, written in C#, is a multiplayer game application, that is designed to run in mobile devices. Using remote procedure calls (RPCs), it propagates a state of the application to player nodes in the network. It might as well have been architected in such a way that it only broadcasts "action" to the participating nodes then have them each calculate the next state. Other than it being the multi-agent game, TrojanBlitz has an architecture typically expected of a typical game application. GameManager, Character, Card, Player are fundamental classes that are designed to scale it further.
  </p>
  <h2>
    ⚠️WARNING - Uncontrolled Hot Loop
  </h2>
  <p>
    This game application is not production ready. It is not optimized enough to spend just as much computational resources as it needs to run. It is therefore not recommended to keep this website (app) open after playing the game.
  </p>
  <h2>
    CSCI599
  </h2>
  <p>
    This project is initiated as a course assignment of CSCI599 - Special Topics (Social Mobile Games), at the University of Southern California in Spring 2020.
  </p>
  <ul>
    <li>Everett Frank	<a href="mailto:everettf@usc.edu">everettf@usc.edu</a></li>
    <li>Zhicheng Jin <a href="mailto:jinzhich@usc.edu">jinzhich@usc.edu</a></li>
    <li>Elden Park <a href="mailto:parkseun@use.edu">parkseun@use.edu</a></li>
    <li>Zhaoyin Qin <a href="mailto:jiarongq@usc.edu">jiarongq@usc.edu</a></li>
    <li>Sy Suo <a href="mailto:yuansuo@usc.edu">yuansuo@usc.edu</a></li>
    <li>Shiqi Ye <a href="mailto:shiquiye@usc.edu">shiquiye@usc.edu</a></li>
  </ul>
</div>
`;

  $('title').html('TrojanBlitz: Fast deck building card game');
  $('head').append(styleHtml);
  $('head').append(headHtml);
  $('head').append(scriptHtml);
  $('body').prepend('<div id="wrap"></div>');
  const webglContent = $('.webgl-content');
  $('#unityContainer').attr('style', `width: ${GameContainer.width}; height: ${GameContainer.height}; max-width: ${GameContainer.maxWidth}; max-height: ${GameContainer.maxHeight};`);

  $('#wrap').prepend(warnHtml);
  $('#wrap').append(webglContent);
  $('#wrap').append(descHtml);
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
