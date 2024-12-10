function enableEditing() {
    const bioP = document.getElementById('authorBio');
    const editButton = document.getElementById('editButton');
    const editInput = document.getElementById('editInput');
    const saveButton = document.getElementById('saveButton');
    const form = document.getElementsByClassName('bio-form')[0];

    // Hide the text and the edit button
    bioP.style.display = 'none';
    form.style.display = 'flex';
    editButton.style.display = 'none';

    // Focus on the input field
    editInput.focus();

    // Place the cursor at the end of the text
    const length = editInput.value.length;
    editInput.setSelectionRange(length, length);
}