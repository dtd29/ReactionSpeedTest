mergeInto(LibraryManager.library, {

  StartGoogleLogin: function () {
    if (typeof StartGoogleLoginJS === 'function') {
      StartGoogleLoginJS();
    } else {
      console.error("StartGoogleLoginJS 함수가 정의되지 않았습니다.");
    }
  },

  SaveScoreToFirestore: function (uidPtr, namePtr, score) {
    const uid = UTF8ToString(uidPtr);
    const name = UTF8ToString(namePtr);

    if (typeof SaveScoreToFirestoreJS === 'function') {
      SaveScoreToFirestoreJS(uid, name, score);
    } else {
      console.error("SaveScoreToFirestoreJS 함수가 정의되지 않았습니다.");
    }
  },
  
  LoadScoreFromFirestore: function (uidPtr) {
    const uid = UTF8ToString(uidPtr);
    if (typeof LoadScoreFromFirestoreJS === 'function') {
      LoadScoreFromFirestoreJS(uid);
    } else {
      console.error("LoadScoreFromFirestoreJS 함수가 정의되지 않았습니다.");
    }
  }

});