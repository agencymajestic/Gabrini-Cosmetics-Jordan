/*!
 * jQuery imagesLoaded plugin v2.1.1
 * http://github.com/desandro/imagesloaded
 *
 * MIT License. by Paul Irish et al.
 */
(function(n,t){"use strict";var i="data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";n.fn.imagesLoaded=function(r){function l(){var t=n(h),i=n(o);u&&(o.length?u.reject(f,t,i):u.resolve(f));n.isFunction(r)&&r.call(e,f,t,i)}function a(n){c(n.target,n.type==="error")}function c(t,r){t.src!==i&&n.inArray(t,s)===-1&&(s.push(t),r?o.push(t):h.push(t),n.data(t,"imagesLoaded",{isBroken:r,src:t.src}),v&&u.notifyWith(n(t),[r,f,n(h),n(o)]),f.length===s.length&&(setTimeout(l),f.unbind(".imagesLoaded",a)))}var e=this,u=n.isFunction(n.Deferred)?n.Deferred():0,v=n.isFunction(u.notify),f=e.find("img").add(e.filter("img")),s=[],h=[],o=[];return n.isPlainObject(r)&&n.each(r,function(n,t){n==="callback"?r=t:u&&u[n](t)}),f.length?f.bind("load.imagesLoaded error.imagesLoaded",a).each(function(r,u){var e=u.src,f=n.data(u,"imagesLoaded");if(f&&f.src===e){c(u,f.isBroken);return}if(u.complete&&u.naturalWidth!==t){c(u,u.naturalWidth===0||u.naturalHeight===0);return}(u.readyState||u.complete)&&(u.src=i,u.src=e)}):l(),u?u.promise(e):e}})(jQuery);
