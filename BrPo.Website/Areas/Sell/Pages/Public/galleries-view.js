document.addEventListener( 'DOMContentLoaded', function () {
    $( "body" ).addClass( "full-screen" );
    $( "#main_layout_container" ).removeClass( "container" ).addClass( "container-fluid" );
    $( "#main_layout_container" ).removeClass( "container" ).addClass( "p-0" );
    $( "main" ).addClass( "full-height" ).removeClass( "pb-3" );
    $( ".container-fluid" ).addClass( "full-height" );
    $( "nav" ).removeClass( "mb-3" );
    //window.addEventListener('resize', resizeCarousel);
    // load functions
    var viewHeight = 1080;
    var viewWidth = 1920;

    function loadGalleryCarousel( galleryRootName, galleryName ) {
        try {
            if ( $( "carousel-image-container" ).length > 0 ) imageHeight = $( "carousel-image-container" ).height();
            if ( $( "carousel-image-container" ).length > 0 ) viewWidth = $( "carousel-image-container" ).width();
            $( "#carousel-container" ).load( `GalleryView?handler=GalleryPartial&galleryRootName=${ galleryRootName }&gallName=${ galleryName }&viewHeight=${ viewHeight }&viewWidth=${ viewWidth }`, function ( response, status, xhr ) {
                console.log( `loadGalleryCarousel: status:${ status }` );
                if ( status == "error" ) {
                    alertError( `Error from loadGalleryCarousel.error: ${ response }` )
                }
                if ( status == "success" ) {
                    try {
                        footer_autohide = document.querySelector( 'footer.autohide' );
                        footer_autohide.classList.add( 'footer-scrolled-down' );
                        bindCarouselControls();
                        bindTooltips();
                        bindKeyboardInput();
                        showTimer = setTimeout( mouseStopped, 5000 );
                        let navbarspacerheight = ( $( "nav" ).height() + 19 ) + 'px';
                        $( ".carousel-content-navbar-spacer" ).css( 'height', navbarspacerheight ).css( 'min-height', navbarspacerheight );
                        cssVar( 'nav-bar-height', $( "nav" ).height() + 'px' );
                        cssVar( 'indicators-height', $( "#carousel-indicators-flex-div" ).height() + 'px' );
                    }
                    catch ( e ) {
                        alertError( `Error from loadGalleryCarousel.success: ${ e.name } : ${ e.description } : ${ e.message }` )
                    }
                    finally {
                    }
                }
            } );
        }
        catch ( e ) {
            alertError( `Error from loadGalleryCarousel: ${ e.name } : ${ e.description } : ${ e.message }` );
        }
        finally {
            showLoader( false );
        }
    }

    function bindCarouselControls() {
        $( ".carousel-control-show-hide-menu div i" ).on( 'mousedown', function () {
            if ( $( "nav" ).hasClass( 'scrolled-down' ) ) {
                showMenu();
            } else {
                hideMenu();
            }
        } );
        $( ".carousel-control-show-hide-indicators div i" ).on( 'mousedown', function () {
            if ( $( "#carousel-indicators-flex-div" ).hasClass( 'carousel-indicators-flex-div-scrolled-down' ) ) {
                showIndicators();
            } else {
                hideIndicators();
            }
        } );
        $( ".carousel-control-scroll-left" ).on( 'click', function () {
            moveIndicatorsLeft();
        } );
        $( ".carousel-control-scroll-right" ).on( 'click', function () {
            moveIndicatorsRight();
        } );
        $( ".carousel-control-full-screen" ).on( 'click', function () {
            if ( $( ".carousel-control-full-screen i" ).hasClass( 'fa-expand' ) ) {
                $( ".carousel-control-full-screen i" ).removeClass( 'fa-expand' ).addClass( 'fa-compress' );
                hideMenu();
                document.documentElement.requestFullscreen();
            } else {
                $( ".carousel-control-full-screen i" ).removeClass( 'fa-compress' ).addClass( 'fa-expand' );
                document.exitFullscreen();
                showMenu();
            }
        } );
        $( ".carousel-control-play-pause" ).on( 'click', function () {
            playOrPause();
        } );
        bindSettingsControls();
    }

    function playOrPause() {
        if ( $( ".carousel-control-play-pause i" ).hasClass( 'fa-pause' ) ) {
            $( ".carousel-control-play-pause i" ).removeClass( 'fa-pause' ).addClass( 'fa-play' );
            carousel.carousel( 'pause' );
        } else {
            $( ".carousel-control-play-pause i" ).removeClass( 'fa-play' ).addClass( 'fa-pause' );
            carousel.carousel( 'cycle' );
        }
    }

    var carousel;

    function bindSettingsControls() {
        $( ".carousel-control-show-settings" ).on( 'click', function ( e ) {
            if ( !$( "#settingsModal" ).hasClass( "show" ) ) {
                $( "#settingsModal" ).modal( 'show' );
            }
        } );

        $( "#duration" ).on( "slide change", function ( slideEvt ) {
            setCarouselTransitionTime( slideEvt.value );
        } );

        carousel = $( '#gallery-carousel' ).carousel( {
            interval: 5000,
            wrap: true,
            ride: 'carousel',
            pause: false,
            cycle: 'reverse'
        } );

        $( "#interval" ).on( "slide change", function ( slideEvt ) {
            var multiplier = $( 'input[type=radio][name=animation]' ).val() == 'fade' ? 1000 : 100;
            var interval = ( slideEvt.value * multiplier );
            setCarouselInterval( interval );
        } );

        $( 'input[type=radio][name=animation]' ).change( function () {
            if ( this.value == 'fade' ) {
                setCarouselInterval( 5000 );
                setCarouselTransitionTime( 5 );
                carousel.addClass( 'carousel-fade' );
            }
            else if ( this.value == 'slide' ) {
                setCarouselInterval( 5000 );
                setCarouselTransitionTime( 7 );
                carousel.removeClass( 'carousel-fade' );
            }
        } );

        $( "#show-name" ).on( 'change', function () {
            if ( $( this ).prop( 'checked' ) ) {
                $( ".carousel-item-name" ).fadeIn();
            } else {
                $( ".carousel-item-name" ).fadeOut();
            }
        } );

        $( "#show-description" ).on( 'change', function () {
            if ( $( this ).prop( 'checked' ) ) {
                $( ".carousel-item-description" ).fadeIn();
            } else {
                $( ".carousel-item-description" ).fadeOut();
            }
        } );
    }

    function setCarouselTransitionTime( value ) {
        let inTime = value / 100;
        let outTime = value / 100;
        if ( $( 'input[type=radio][name=animation]' ).val() == 'fade' ) {
            inTime *= 10;
            outTime *= 10;
            inTime -= ( parseFloat( inTime ) / 5 );
            outTime += ( parseFloat( inTime ) / 5 );
        }
        cssVar( 'slide-transition-in-time', inTime + 's' );
        cssVar( 'slide-transition-out-time', outTime + 's' );
    }

    function setCarouselInterval( intervalMs ) {
        config = carousel.data()[ 'bs.carousel' ]._config
        config.interval = intervalMs;
        carousel.data( { _config: config } )
    }

    $( '#duration' ).slider( {
        formatter: function ( value ) {
            return value;
        }
    } );

    $( '#interval' ).slider( {
        formatter: function ( value ) {
            return value;
        }
    } );

    function bindKeyboardInput() {
        $( document ).keydown( function ( event ) {
            if ( event.which === 37 ) {
                moveCarouselLeft();
            }
        } );
        $( document ).keydown( function ( event ) {
            if ( event.which === 39) {
                moveCarouselRight();
            }
        } );
        $( document ).keydown( function ( event ) {
            if (event.which === 32 ) {
                playOrPause();
            }
        } );
    }

    $( function () {
        var n = 0;
        $( '#bg-colour' ).colorpicker( {
            debug: false,
            extensions: [
                {
                    name: 'swatches',
                    options: {
                        colors: {
                            'black': '#000000',
                            'dark gray': '#888888',
                            'gray': '#CCCCCC',
                            'light gray': '#F0F0F0',
                            'white': '#ffffff',
                            'taup': '#CAC4BE',
                            'blue is the colour': '#034694'
                        },
                        namesAsValues: true
                    }
                }
            ]
        } )
            .on( 'colorpickerDebug', function ( e ) {
                console.log( n + ': ' + e.debug.eventName + '' );
            } )
            .on( 'colorpickerChange ', function ( e ) {
                $( "#displayPanelRoom" ).css( 'background', e.color.toString() );
                $( ".colorpicker-element input" ).css( 'color', e.color.toString() );
                setTimeout(
                    $( ".colorpicker-element input" ).css( 'border', '0 !important' ),
                    20
                );
                $( "#carousel-container" ).css( 'background', e.color.toString() );
            } );
    } );

    //function resizeCarousel() {
    //    //$(".carousel-container").height(window.innerHeight - $(".carousel-indicators").height());
    //    //$(".carousel-container").css("min-height", $(".carousel-container").height());
    //}

    // initialisation

    function moveIndicatorsLeft() {
        $( ".carousel-indicators" ).animate( { scrollLeft: -$( ".carousel-indicators" ).width() }, 800 );
    }

    function moveIndicatorsRight() {
        let containerWidth = $( ".carousel-indicators" ).width();
        let offset = $( ".carousel-indicators" ).scrollLeft();
        let scrollAmount = containerWidth - offset < containerWidth ? containerWidth - offset : containerWidth;
        $( ".carousel-indicators" ).animate( { scrollLeft: scrollAmount }, 800 );
    }

    function moveCarouselLeft() {
        $( '.carousel' ).carousel( 'prev' )
    }

    function moveCarouselRight() {
        $( '.carousel' ).carousel( 'next' )
    }

    function showMenu() {
        $( ".carousel-content-navbar-spacer" ).addClass( 'carousel-content-navbar-spacer-scrolled-up' );
        $( ".carousel-content-navbar-spacer" ).removeClass( 'carousel-content-navbar-spacer-scrolled-down' );
        $( "nav" ).addClass( 'scrolled-up' );
        $( "nav" ).removeClass( 'scrolled-down' );
        $( ".carousel-control-show-hide-menu div i" ).addClass( 'fa-angle-up' ).removeClass( 'fa-angle-down' );
    }

    function hideMenu() {
        $( ".carousel-content-navbar-spacer" ).addClass( 'carousel-content-navbar-spacer-scrolled-down' ).css('min-height', '0');
        $( ".carousel-content-navbar-spacer" ).removeClass( 'carousel-content-navbar-spacer-scrolled-up' );
        $( "nav" ).addClass( 'scrolled-down' );
        $( "nav" ).removeClass( 'scrolled-up' );
        $( ".carousel-control-show-hide-menu div i" ).removeClass( 'fa-angle-up' ).addClass( 'fa-angle-down' );
    }

    function showIndicators() {
        showIndicatorNav();
        $( ".carousel-control-show-hide-indicators div i" ).removeClass( 'fa-angle-up' ).addClass( 'fa-angle-down' );
        $( "#carousel-indicators-flex-div" ).addClass( "carousel-indicators-flex-div-scrolled-up" );
        $( "#carousel-indicators-flex-div" ).removeClass( "carousel-indicators-flex-div-scrolled-down" );
    }

    function hideIndicators() {
        hideMenu();
        hideIndicatorNav();
        $( ".carousel-control-show-hide-indicators div i" ).addClass( 'fa-angle-up' ).removeClass( 'fa-angle-down' );
        $( "#carousel-indicators-flex-div" ).addClass( "carousel-indicators-flex-div-scrolled-down" );
        $( "#carousel-indicators-flex-div" ).removeClass( "carousel-indicators-flex-div-scrolled-up" );
    }

    function showIndicatorNav() {
        $( ".carousel-control-scroll-left" ).fadeIn( 500 );
        $( ".carousel-control-scroll-right" ).fadeIn( 500 );
    }

    function hideIndicatorNav() {
        $( ".carousel-control-scroll-left" ).fadeOut( 500 );
        $( ".carousel-control-scroll-right" ).fadeOut( 500 );
    }

    setTimeout(
        loadGalleryCarousel( galleryRootName, galleryName ),
        150 );

    function addListenerMulti( element, eventNames, listener ) {
        var events = eventNames.split( ' ' );
        for ( var i = 0, iLen = events.length; i < iLen; i++ ) {
            element.addEventListener( events[ i ], listener, false );
        }
    }

    var hideTimer;
    var showTimer;

    addListenerMulti( window, 'mousemove touchmove', function () {
        showControls();
        clearTimeout( hideTimer );
        hideTimer = setTimeout( mouseStopped, 2500 );
    } );

    function mouseStopped() {
        //$( ".carousel-control" ).finish().animate( { opacity: 0 }, 500 );
    }

    function showControls() {
        $( ".carousel-control" ).finish().animate( { opacity: 0.1 }, 200, "swing" );
        $( ".carousel-control" ).finish().animate( { opacity: 1 }, 300, "swing" );
        clearTimeout( showTimer );
        showTimer = setTimeout( mouseStopped, 3500 );
    }
} );
