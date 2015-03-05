window.NotyHelper = (function() {
  function NotyHelper() {}

  NotyHelper.prototype.generate = function(type, title, content) {
    var icon, n, text;
    icon = '';
    if (type === 'error') {
      icon = 'fa-bolt';
    } else if (type === 'success') {
      icon = 'fa-thumbs-o-up';
    } else if (type === 'info') {
      icon = 'fa-info';
    }
    text = '<div class="noty_title"><i class="fa ' + icon + '"></i>' + title + '</div><div class="noty_content">' + content + '</div>';
    n = noty({
      text: text,
      type: type,
      dismissQueue: true,
      layout: 'topLeft',
      closeWith: ['click'],
      theme: 'relax',
      maxVisible: 10,
      animation: {
        open: 'animated fadeInLeft',
        close: 'animated fadeOutLeft',
        easing: 'swing',
        speed: 100
      }
    });
    return setTimeout((function() {
      return $('#' + n.options.id).trigger('click');
    }), 3000);
  };

  return NotyHelper;

})();
