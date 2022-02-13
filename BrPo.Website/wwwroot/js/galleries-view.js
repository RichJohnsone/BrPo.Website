document.addEventListener( 'DOMContentLoaded', function () {
    $( "body" ).addClass( "full-screen" );
    $( "#main_layout_container" ).removeClass( "container" ).addClass( "container-fluid" );
    $( "#main_layout_container" ).removeClass( "container" ).addClass( "p-0" );
    $( "main" ).addClass( "full-height" ).removeClass( "pb-3" );
    $( ".container-fluid" ).addClass( "full-height" );
    //window.addEventListener('resize', resizeCarousel);
    // load functions
    var viewHeight = 700;
    var viewWidth = 1200;

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
                        $( ".carousel-content-navbar-spacer" ).css( 'height', ( $( "nav" ).height() + 47 ) + 'px' );
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
        bindSettingsControl();
    }

    function bindSettingsControl() {
        $( ".carousel-control-show-settings" ).on( 'click', function ( e ) {
            if ( !$( "#settingsModal" ).hasClass( "show" ) ) {
                $( "#settingsModal" ).modal( 'show' );
            }
        } );
    }

    function bindKeyboardInput() {
        $( document ).keydown( function ( event ) {
            if ( event.which === 37 ) {
                moveCarouselLeft();
            }
        } );
        $( document ).keydown( function ( event ) {
            if ( event.which === 39 || event.which === 32 ) {
                moveCarouselRight();
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

    $( '#duration' ).slider( {
        formatter: function ( value ) {
            return 'Current value: ' + value;
        }
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
        $( ".carousel-indicators" ).animate( { scrollLeft: $( ".carousel-indicators" ).width() }, 800 );
    }

    function moveCarouselLeft() {
        $( '.carousel' ).carousel('prev')
    }

    function moveCarouselRight() {
        $( '.carousel' ).carousel('next')
    }

    function showMenu() {
        $( ".carousel-content-navbar-spacer" ).addClass( 'carousel-content-navbar-spacer-scrolled-up' );
        $( ".carousel-content-navbar-spacer" ).removeClass( 'carousel-content-navbar-spacer-scrolled-down' );
        $( "nav" ).addClass( 'scrolled-up' );
        $( "nav" ).removeClass( 'scrolled-down' );
        $( ".carousel-control-show-hide-menu div i" ).addClass( 'fa-angle-up' ).removeClass( 'fa-angle-down' );
    }

    function hideMenu() {
        $( ".carousel-content-navbar-spacer" ).addClass( 'carousel-content-navbar-spacer-scrolled-down' );
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

    var hideTimer;
    var showTimer;
    var lastCalled = 0;

    window.addEventListener( "mousemove", function () {
        showControls();
        clearTimeout( hideTimer );
        hideTimer = setTimeout( mouseStopped, 3000 );
    } );

    function mouseStopped() {
        $( ".carousel-control" ).finish().animate( { opacity: 0 }, 500 );
    }

    function showControls() {
        $( ".carousel-control" ).finish().animate( { opacity: 0.1 }, 200, "swing" );
        $( ".carousel-control" ).finish().animate( { opacity: 1 }, 300, "swing");
        clearTimeout( showTimer );
        showTimer = setTimeout( mouseStopped, 5000 );
    }
} );

const cssVar = ( name, value ) => {
    if ( name.substr( 0, 2 ) !== "--" ) {
        name = "--" + name;
    }
    if ( value ) {
        document.documentElement.style.setProperty( name, value )
    }
    return getComputedStyle( document.documentElement ).getPropertyValue( name );
}