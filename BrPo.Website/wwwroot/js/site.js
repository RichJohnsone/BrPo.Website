// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$( document ).ready( function () {
    $( '[data-toggle="tooltip"]' ).tooltip();
    setTimeout( bindTooltips, 1000 );
    updateBasketCount();
} );

var tooltipHideTimer;

function bindTooltips() {
    $( '[data-toggle="tooltip"]' ).tooltip( {
        delay: { show: 300, hide: 300 }
    } ).click( function () {
        $( '[data-toggle="tooltip"]' ).tooltip( "hide" );
    } );
    //$('[data-toggle="tooltip"]').on('shown.bs.tooltip', function (){
    //    clearTimeout( tooltipHideTimer );
    //    tooltipHideTimer = setTimeout($('[data-toggle="tooltip"]').tooltip("hide"), 4000);
    //});
}

function getCurrentFormId( htmlElement ) {
    var inputElement = htmlElement.currentTarget;
    var form = $( inputElement ).closest( 'form' )[ 0 ];
    var formId = $( form ).attr( "id" );
    return formId;
}

function updateBasketCount() {
    try {
        $.ajax( {
            url: '/Print/Order?handler=BasketCount',
            success: function ( response ) {
                let count = response == 0 ? '' : response;
                $( "#shoppingBasketCount" ).html( count );
                if ( count > 0 ) {
                    $( "#shopping-basket-nav" ).show();
                } else {
                    $( "#shopping-basket-nav" ).hide();
                }
            },
            error: function ( data ) {
                console.log( `Ajax error from getBasketCount: status = ${ data.status } : description: ${ data.statusText }` );
            }
        } );
    }
    catch ( e ) {
        console.log( `Error from updateBasketCount : ${ e.name } :  : ${ e.description } : ${ e.message }` );
    }
}

function showLoader( show ) {
    if ( show || show == undefined ) {
        $( '#spinnerModal' ).modal( 'show' );
        $( 'body' ).removeClass( 'modal-open' );
    } else {
        $( '#spinnerModal' ).modal( 'hide' );
    }
}

function alertSuccess( message ) {
    $( "#alert-container" ).show().removeClass( "alert-danger" ).addClass( "alert-success" ).removeClass( "alert-warning" );
    $( "#alert-icon" ).removeClass().addClass( "fas fa-check-circle fa-lg" );
    $( "#alert-message" ).html( `Success:   ${ message }` );
}

function alertWarning( message ) {
    $( "#alert-message" ).show().removeClass( "alert-danger" ).removeClass( "alert-success" ).addClass( "alert-warning" );
    $( "#alert-icon" ).removeClass().addClass( "fas fa-exclamation-circle fa-lg" );
    $( "#alert-message" ).html( `Warning:   ${ message }` );
    console.log( message );
}

function alertError( message ) {
    $( "#alert-container" ).show().addClass( "alert-danger" ).removeClass( "alert-success" ).removeClass( "alert-warning" );
    $( "#alert-icon" ).removeClass().addClass( "fas fa-exclamation-circle fa-lg" );
    $( "#alert-message" ).html( `Error:   ${ message }` );
    console.log( message );
}

function alertClear() {
    $( "#alert-container" ).hide();
    $( "#alert-message" ).html();
}

// autohide nav and footer

nav_autohide = document.querySelector( 'nav.autohide-js' );
footer_autohide = document.querySelector( 'footer.autohide-js' );
// add padding-top to body (if necessary)
navbar_height = document.querySelector( '.navbar' ).offsetHeight;
document.body.style.paddingTop = navbar_height + 'px';

if ( nav_autohide ) {
    var last_scroll_top = 0;
    window.addEventListener( 'scroll', function () {
        let scroll_top = window.scrollY;
        if ( scroll_top < last_scroll_top ) {
            nav_autohide.classList.remove( 'scrolled-down' );
            nav_autohide.classList.add( 'scrolled-up' );
            footer_autohide.classList.remove( 'footer-scrolled-down' );
            footer_autohide.classList.add( 'footer-scrolled-up' );
        }
        else {
            nav_autohide.classList.remove( 'scrolled-up' );
            nav_autohide.classList.add( 'scrolled-down' );
            footer_autohide.classList.remove( 'footer-scrolled-up' );
            footer_autohide.classList.add( 'footer-scrolled-down' );
        }
        last_scroll_top = scroll_top;
    } );
}

const cssVar = ( name, value ) => {
    if ( name.substr( 0, 2 ) !== "--" ) {
        name = "--" + name;
    }
    if ( value ) {
        document.documentElement.style.setProperty( name, value )
    }
    return getComputedStyle( document.documentElement ).getPropertyValue( name );
}