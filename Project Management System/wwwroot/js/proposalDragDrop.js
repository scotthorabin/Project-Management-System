$(document).ready(function () {
    var draggedItem = null;
    var isAnimating = false;

    initializePage();

    $('.body--submitLevel6--container .TopicsBtn, .body--submitLevel7--container .TopicsBtn').click(function (e) {
        e.preventDefault();
        if (!isAnimating) {
            toggleSections($(this), false);
        }
    });

    $('.body--submitLevel6--container .FormUploadBtn, .body--submitLevel7--container .FormUploadBtn').click(function (e) {
        e.preventDefault();
        if (!isAnimating) {
            toggleSections($(this), true);
        }
    });

    function toggleSections(button, isUpload) {
        var $container = button.closest('div[class^="body--submitLevel"]');
        isAnimating = true; // Lock interactions

        if (isUpload) {
            $container.find('.topicProposalContainer, .proposalsPriority').slideUp(400, function () {
                $container.find('.formUploadContainer').slideDown(400, function () {
                    isAnimating = false; // Unlock interactions after animation
                });
            });
            button.css('border-bottom', '2px solid red');
            $container.find('.TopicsBtn').css('border', 'none');
        } else {
            $container.find('.formUploadContainer').slideUp(400, function () {
                $container.find('.topicProposalContainer, .proposalsPriority').slideDown(400, function () {
                    isAnimating = false; // Unlock interactions after animation
                });
            });
            button.css('border-bottom', '2px solid red');
            $container.find('.FormUploadBtn').css('border', 'none');
        }
    }

    // Initialize default page state
    function initializePage() {
        $('.body--submitLevel6--container .TopicsBtn, .body--submitLevel7--container .TopicsBtn').css('border-bottom', '2px solid red');
        $('.formUploadContainer').hide();
        $('.proposalsPriority, .topicProposalContainer').show();
    }

    $('.incorrectDetails').click(function () {
        $('.additionalInfo').slideToggle();
    });

    $('.proposalBox').on('dragstart', function (e) {
        draggedItem = this;
        setTimeout(() => $(this).hide(), 0);
    });

    $('.proposalBox').on('dragend', function (e) {
        setTimeout(() => $(this).show(), 0);
        draggedItem = null;
    });

    $('.proposalBox').on('dragover', function (e) {
        e.preventDefault();
    });

    $('.proposalBox').on('dragenter', function (e) {
        e.preventDefault();
        $(this).addClass('over');
    });

    $('.proposalBox').on('dragleave', function (e) {
        $(this).removeClass('over');
    });

    $('.proposalBox').on('drop', function (e) {
        $(this).removeClass('over');
        if (!draggedItem) return;
        const targetRect = this.getBoundingClientRect();
        const targetMidpoint = targetRect.top + targetRect.height / 2;
        const afterTarget = e.originalEvent.clientY > targetMidpoint;
        if (afterTarget && this.nextSibling) {
            $(draggedItem).insertAfter($(this));
        } else {
            $(draggedItem).insertBefore($(this));
        }
    });
});
