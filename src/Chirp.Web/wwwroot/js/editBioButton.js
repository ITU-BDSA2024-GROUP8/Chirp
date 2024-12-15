function enableEditing() {
    const bioP = document.getElementById('authorBio');
    const editButton = document.getElementById('editButton');
    const editInput = document.getElementById('editInput');
    const form = document.getElementsByClassName('bio-form')[0];
    const cancelButton = document.getElementById('cancel-button');
    
    // Hide the text and the edit button
    bioP.style.display = 'none';
    editButton.style.display = 'none';
    form.style.display = 'flex';
    cancelButton.style.display = 'flex';

    // Focus on the input field
    editInput.focus();

    // Place the cursor at the end of the text
    const length = editInput.value.length;
    editInput.setSelectionRange(length, length);
}