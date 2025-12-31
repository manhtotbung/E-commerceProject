'use strict';

(function ($) {

    /*------------------
        Preloader
    --------------------*/
    $(window).on('load', function () {
        $(".loader").fadeOut();
        $("#preloder").delay(200).fadeOut("slow");

        /*------------------
            Product filter
        --------------------*/
        $('.filter__controls li').on('click', function () {
            $('.filter__controls li').removeClass('active');
            $(this).addClass('active');
        });
        if ($('.property__gallery').length > 0) {
            var containerEl = document.querySelector('.property__gallery');
            var mixer = mixitup(containerEl);
        }
    });

    /*------------------
        Background Set
    --------------------*/
    $('.set-bg').each(function () {
        var bg = $(this).data('setbg');
        $(this).css('background-image', 'url(' + bg + ')');
    });

    //Search Switch
    $('.search-switch').on('click', function () {
        $('.search-model').fadeIn(400);
    });

    $('.search-close-switch').on('click', function () {
        $('.search-model').fadeOut(400, function () {
            $('#search-input').val('');
        });
    });

    //Canvas Menu
    $(".canvas__open").on('click', function () {
        $(".offcanvas-menu-wrapper").addClass("active");
        $(".offcanvas-menu-overlay").addClass("active");
    });

    $(".offcanvas-menu-overlay, .offcanvas__close").on('click', function () {
        $(".offcanvas-menu-wrapper").removeClass("active");
        $(".offcanvas-menu-overlay").removeClass("active");
    });

    /*------------------
		Navigation
	--------------------*/
    $(".header__menu").slicknav({
        prependTo: '#mobile-menu-wrap',
        allowParentLinks: true
    });

    /*------------------
        Accordin Active
    --------------------*/
    $('.collapse').on('shown.bs.collapse', function () {
        $(this).prev().addClass('active');
    });

    $('.collapse').on('hidden.bs.collapse', function () {
        $(this).prev().removeClass('active');
    });

    /*--------------------------
        Banner Slider
    ----------------------------*/
    $(".banner__slider").owlCarousel({
        loop: true,
        margin: 0,
        items: 1,
        dots: true,
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: true
    });

    /*--------------------------
        Product Details Slider
    ----------------------------*/
    $(".product__details__pic__slider").owlCarousel({
        loop: true,
        margin: 0,
        items: 1,
        dots: false,
        nav: true,
        navText: ["<i class='bi bi-chevron-left'></i>","<i class='bi bi-chevron-right'></i>"],
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: false,
        mouseDrag: false,
        startPosition: 'URLHash'
    }).on('changed.owl.carousel', function(event) {
        var indexNum = event.item.index + 1;
        product_thumbs(indexNum);
    });

    function product_thumbs (num) {
        var thumbs = document.querySelectorAll('.product__thumb a');
        thumbs.forEach(function (e) {
            e.classList.remove("active");
            if(e.hash.split("-")[1] == num) {
                e.classList.add("active");
            }
        })
    }


    /*------------------
		Magnific
    --------------------*/
    $('.image-popup').magnificPopup({
        type: 'image'
    });


    $(".nice-scroll").niceScroll({
        cursorborder:"",
        cursorcolor:"#dddddd",
        boxzoom:false,
        cursorwidth: 5,
        background: 'rgba(0, 0, 0, 0.2)',
        cursorborderradius:50,
        horizrailenabled: false
    });

    /*------------------
        CountDown
    --------------------*/
    // For demo preview start
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();

    if(mm == 12) {
        mm = '01';
        yyyy = yyyy + 1;
    } else {
        mm = parseInt(mm) + 1;
        mm = String(mm).padStart(2, '0');
    }
    var timerdate = mm + '/' + dd + '/' + yyyy;
    // For demo preview end


    // Uncomment below and use your date //

    /* var timerdate = "2020/12/30" */

	$("#countdown-time").countdown(timerdate, function(event) {
        $(this).html(event.strftime("<div class='countdown__item'><span>%D</span> <p>Day</p> </div>" + "<div class='countdown__item'><span>%H</span> <p>Hour</p> </div>" + "<div class='countdown__item'><span>%M</span> <p>Min</p> </div>" + "<div class='countdown__item'><span>%S</span> <p>Sec</p> </div>"));
    });

    /*-------------------
		Range Slider
	--------------------- */
	var rangeSlider = $(".price-range"),
    minamount = $("#minamount"),
    maxamount = $("#maxamount"),
    minPrice = rangeSlider.data('min'),
    maxPrice = rangeSlider.data('max');
    rangeSlider.slider({
    range: true,
    min: minPrice,
    max: maxPrice,
    values: [minPrice, maxPrice],
    slide: function (event, ui) {
        minamount.val('$' + ui.values[0]);
        maxamount.val('$' + ui.values[1]);
        }
    });
    minamount.val('$' + rangeSlider.slider("values", 0));
    maxamount.val('$' + rangeSlider.slider("values", 1));

    /*------------------
		Single Product
	--------------------*/
	$('.product__thumb .pt').on('click', function(){
		var imgurl = $(this).data('imgbigurl');
		var bigImg = $('.product__big__img').attr('src');
		if(imgurl != bigImg) {
			$('.product__big__img').attr({src: imgurl});
		}
    });
    
    /*-------------------
		Quantity change
	--------------------- */
    $(function () {
        const nf = new Intl.NumberFormat('vi-VN');

        // Gỡ mọi bind cũ 
        $(document).off('click', '.pro-qty .qtybtn');
        $(document).off('change', '.pro-qty input');

        // Tính lại tiền của 1 dòng + tổng giỏ
        function recalc($row) {
            const qty = parseInt($row.find('.pro-qty input').val(), 10) || 0;
            let unit = Number($row.data('unit'));
            if (!unit) {
                const txt = $row.find('td.cart__price').last().text(); 
                unit = parseInt(txt.replace(/[^\d]/g, '')) || 0;       
            }
            const line = unit * qty;
            $row.find('.cart__total').text(nf.format(line) + ' vnd');

            // tổng giỏ
            let grand = 0;
            $('tr.cart-row').each(function () {
                const r = $(this);
                const u = Number(r.data('unit')) ||
                    parseInt(r.find('td.cart__price').last().text().replace(/[^\d]/g, '')) || 0;
                const q = parseInt(r.find('.pro-qty input').val(), 10) || 0;
                grand += u * q;
            });
            $('#cartTotal').text(nf.format(grand) + ' vnd');
        }

        // Handler DUY NHẤT cho +/-
        $(document).on('click', '.pro-qty .qtybtn', function (e) {
            e.preventDefault();
            e.stopPropagation();

            const $row = $(this).closest('tr.cart-row');
            const $input = $row.find('.pro-qty input');
            let val = parseInt($input.val(), 10) || 0;

            val = $(this).hasClass('inc') ? val + 1 : Math.max(0, val - 1);
            $input.val(val);

            recalc($row);
        });

        // Nhập tay rồi rời ô
        $(document).on('change blur', '.pro-qty input', function () {
            const $row = $(this).closest('tr.cart-row');
            let v = parseInt(this.value, 10);
            if (isNaN(v) || v < 0) v = 0;
            $(this).val(v);
            recalc($row);
        });
    });

    //   var proQty = $('.pro-qty');
	////proQty.prepend('<span class="dec qtybtn">-</span>');
	////proQty.append('<span class="inc qtybtn">+</span>');
	//proQty.on('click', '.qtybtn', function () {
	//	var $button = $(this);
	//	var oldValue = $button.parent().find('input').val();
	//	if ($button.hasClass('inc')) {
	//		var newVal = parseFloat(oldValue) + 1;
	//	} else {
	//		// Don't allow decrementing below zero
	//		if (oldValue > 0) {
	//			var newVal = parseFloat(oldValue) - 1;
	//		} else {
	//			newVal = 0;
	//		}
	//	}
	//	$button.parent().find('input').val(newVal);
 //   });

 //   var proQty = $('.pro-qty');
 //   const nf = new Intl.NumberFormat('vi-VN');

 //   // Cập nhật tiền của 1 dòng + tổng giỏ
 //   function recalc($btn) {
 //       var $row = $btn.closest('tr.cart-row');
 //       var qty = parseInt($row.find('.pro-qty input').val(), 10) || 0;

 //       // đơn giá số: ưu tiên lấy từ data-unit, fallback parse từ text
 //       var unit = Number($row.data('unit'));
 //       if (!unit) {
 //           var txt = $row.find('td.cart__price').last().text(); // ví dụ "499.000 vnd"
 //           unit = Number(txt.replace(/[^\d]/g, ''));            // -> 499000
 //       }

 //       var line = unit * qty;
 //       $row.find('.cart__total').text(nf.format(line) + ' vnd');

 //       // tính lại tổng giỏ
 //       var grand = 0;
 //       $('tr.cart-row').each(function () {
 //           var u = Number($(this).data('unit')) ||
 //               Number($(this).find('td.cart__price').last().text().replace(/[^\d]/g, ''));
 //           var q = parseInt($(this).find('.pro-qty input').val(), 10) || 0;
 //           grand += u * q;
 //       });
 //       $('#cartTotal').text(nf.format(grand) + ' vnd');
 //   }

 //   proQty.on('click', '.qtybtn', function () {
 //       var $button = $(this);
 //       var $input = $button.parent().find('input');
 //       var oldVal = parseFloat($input.val()) || 0;

 //       var newVal = $button.hasClass('inc') ? oldVal + 1 : Math.max(0, oldVal - 1);
 //       $input.val(newVal);

 //       // cập nhật tiền
 //       recalc($button);
 //   });



    //$(function () {
    //    const nf = new Intl.NumberFormat('vi-VN');

    //    function getToken() {
    //        return $('#cartAf input[name="__RequestVerificationToken"]').val();
    //    }

    //    // ===== 1) Quantity change trong Cart =====
    //    // (giữ .pro-qty có sẵn; chỉ thêm handler gọi server + cập nhật UI)
    //    $(document).on('click', '.pro-qty .qtybtn', function (e) {
    //        e.preventDefault();
    //        const $row = $(this).closest('.cart-row');
    //        const $input = $row.find('.pro-qty input');
    //        let val = parseInt($input.val(), 10) || 0;
    //        val = $(this).hasClass('inc') ? val + 1 : Math.max(0, val - 1);
    //        postUpdate($row, val);
    //    });

    //    $(document).on('change blur', '.pro-qty input', function () {
    //        const $row = $(this).closest('.cart-row');
    //        let val = parseInt(this.value, 10);
    //        if (isNaN(val) || val < 0) val = 0;
    //        postUpdate($row, val);
    //    });

    //    function postUpdate($row, qty) {
    //        const id = $row.data('id');
    //        $.ajax({
    //            url: '/Cart/UpdateQuantity',
    //            type: 'POST',
    //            data: { id: id, quantity: qty, __RequestVerificationToken: getToken() },
    //            headers: { 'X-Requested-With': 'XMLHttpRequest' }
    //        })
    //            .done(function (res) {
    //                if (!res || !res.ok) return;
    //                if (res.removed) {
    //                    $row.remove();
    //                } else {
    //                    $row.find('.pro-qty input').val(qty);
    //                    $row.find('.cart__total').text(nf.format(res.lineTotal) + ' vnd');
    //                }
    //                // cập nhật tổng
    //                $('#cartTotal').text(nf.format(res.totalAmount) + ' vnd');
    //                // nếu có mini-cart
    //                $('.mini-cart-qty').text(res.totalQty);
    //                $('.mini-cart-amount').text(nf.format(res.totalAmount) + ' vnd');
    //            })
    //            .fail(function (xhr) {
    //                console.error('UpdateQuantity FAIL', xhr.status, xhr.responseText);
    //            });
    //    }

    //    // ===== 2) Add to cart bằng AJAX (không rời trang) =====
    //    // chặn nổi bọt để không mở trang chi tiết
    //    $(document).on('click', '.product__hover a', function (e) { e.stopPropagation(); });

    //    $(document).on('click', 'a.add-to-cart', function (e) {
    //        e.preventDefault();
    //        e.stopPropagation();
    //        const url = this.href;

    //        $.ajax({
    //            url: url,
    //            type: 'GET',
    //            headers: { 'X-Requested-With': 'XMLHttpRequest' }
    //        })
    //            .done(function (res) {
    //                // server trả { ok, totalQty, totalAmount }
    //                if (res && res.ok) {
    //                    $('.mini-cart-qty').text(res.totalQty);
    //                    $('.mini-cart-amount').text(nf.format(res.totalAmount) + ' vnd');
    //                    toast('Đã thêm vào giỏ');
    //                } else {
    //                    toast('Đã thêm vào giỏ', false);
    //                }
    //            })
    //            .fail(function (xhr) {
    //                console.error('AddToCart FAIL', xhr.status, xhr.responseText);
    //                toast('Thêm giỏ thất bại', true);
    //            });
    //    });

    //    function toast(msg, isErr) {
    //        const $t = $(`
    //  <div class="position-fixed end-0 top-0 m-3 z-50" style="min-width:240px">
    //    <div class="alert ${isErr ? 'alert-danger' : 'alert-success'} py-2 px-3 shadow-sm mb-0">${msg}</div>
    //  </div>
    //`);
    //        $('body').append($t);
    //        setTimeout(() => $t.fadeOut(200, () => $t.remove()), 1500);
    //    }
    //});
    /*-------------------
       Add cart Btn
   --------------------- */
    //$(document).on('click', '.add-to-cart', function () {


    //})

    /*-------------------
		Radio Btn
	--------------------- */
    $(".size__btn label").on('click', function () {
        $(".size__btn label").removeClass('active');
        $(this).addClass('active');
    });

})(jQuery);

/*-------------------
        Product Picture Click
    --------------------- */

// 3) Event delegation: click vào ảnh -> đi đến trang chi tiết (không chặn anchor trong khu hover để lightbox chạy)
$('#productList').on('click', '.product__item__pic.clickable', function (e) {
    if ($(e.target).closest('.product__hover a').length) return; // để lightbox/icon hoạt động
    const url = $(this).closest('[data-url]').attr('data-url');
    if (url) window.location.href = url;
});
// Ngăn nổi bọt khi click icon/link trong hover (không preventDefault)
$('#productList').on('click', '.product__hover a', function (e) {
    e.stopPropagation();
});

//document.querySelectorAll('.product__item__pic.clickable').forEach(function (el) {
//    el.addEventListener('click', function (e) {
//        if (e.target.closest('a')) {
//            return;
//        }
//        const productLink = el.closest('[data-url]').getAttribute('data-url');
//        window.location.href = productLink;
//    });
//});

//document.querySelectorAll('.product__hover a').forEach(function (iconLink) {
//    iconLink.addEventListener('click', function (e) {
//        e.stopPropagation();
//    });
//});

/*-------------------
        Filter Products
    --------------------- */
/* ===== Product Filters (final) ===== */

$(function () {

    function applySetBg(scope) {
        var $root = scope ? $(scope) : $(document);
        $root.find('.set-bg').each(function () {
            var bg = $(this).data('setbg');
            if (bg) $(this).css('background-image', 'url(' + bg + ')');
        });
    }
    function initImagePopup(scope) {
        var $root = scope ? $(scope) : $(document);
        if ($.fn.magnificPopup) {
            $root.find('.image-popup').magnificPopup({
                type: 'image',
                closeBtnInside: true,
                mainClass: 'mfp-fade',
                gallery: { enabled: true }
            });
        } else {
            // Fallback: nếu chưa load plugin, tránh mở đè trang
            $root.off('click.fallback', '.image-popup').on('click.fallback', '.image-popup', function (e) {
                e.preventDefault();
                window.open(this.href, '_blank');
            });
        }
    }

    // --- Helpers lấy giá trị đã chọn ---
    function getCheckedValues(name) {
        return $('input[name="' + name + '"]:checked').map(function () {
            return this.value;
        }).get();
    }
    function getSelectedPrice() {
        return $('input[name="PriceRange"]:checked').val() || '';
    }
    function buildQuery(price, sizes, colors) {
        var p = new URLSearchParams();
        if (price) p.set('priceRange', price);
        sizes.forEach(function (s) { p.append('size', s); });
        colors.forEach(function (c) { p.append('color', c); });
        return p.toString();
    }

    // --- Gọi server theo tất cả filter ---
    function reloadProducts() {
        var price = getSelectedPrice();
        var sizes = getCheckedValues('Size');
        var colors = getCheckedValues('Color');

        $.ajax({
            url: '/Product/Index',
            type: 'GET',
            data: {
                // Server nhận List<string> nên cứ gửi lặp key; cần traditional:true
                priceRange: price ? [price] : [],
                size: sizes,
                color: colors
            },
            traditional: true, // => size=S&size=M&color=Red...
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        })
            .done(function (html) {
                $('#productList').html(html);
                applySetBg('#productList');
                initImagePopup('#productList');

                // Cập nhật URL để share/refresh vẫn giữ trạng thái
                var qs = buildQuery(price, sizes, colors);
                history.replaceState({}, '', qs ? ('?' + qs) : location.pathname);
            })
            .fail(function (xhr, status) {
                console.error('[Filter] FAIL', status, xhr.status, xhr.responseText);
            });
    }

    // --- Lắng nghe thay đổi: PriceRange (radio), Size/Color (checkbox) ---
    $(document).on('change', 'input[name="PriceRange"], input[name="Size"], input[name="Color"]', reloadProducts);

    // --- Dự phòng: click label luôn phát sự kiện change đúng cách ---
    $(document).on('click', 'label[for]', function () {
        var id = $(this).attr('for');
        var el = document.getElementById(id);
        if (!el) return;
        if (el.type === 'radio') {
            if (!el.checked) { el.checked = true; $(el).trigger('change'); }
        } else if (el.type === 'checkbox') {
            el.checked = !el.checked;
            $(el).trigger('change');
        }
    });

    // --- Khởi động theo URL hiện tại (nếu có) ---
    var sp = new URLSearchParams(location.search);
    var urlPrice = sp.get('priceRange');
    if (urlPrice) $('input[name="PriceRange"][value="' + urlPrice + '"]').prop('checked', true);

    sp.getAll('size').forEach(function (v) {
        $('input[name="Size"][value="' + v + '"]').prop('checked', true);
    });
    sp.getAll('color').forEach(function (v) {
        $('input[name="Color"][value="' + v + '"]').prop('checked', true);
    });

    if (urlPrice || sp.has('size') || sp.has('color')) {
        reloadProducts();
    }
});


