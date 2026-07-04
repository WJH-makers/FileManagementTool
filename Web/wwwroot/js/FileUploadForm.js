let currentAction = 'scan';

async function handleClick(event) {
    event.preventDefault();
    const actionDescription = document.getElementById('actionInput').value.trim();

    if (actionDescription) {
        try {
            const actionResult = await callNLPApi(actionDescription);

            if (actionResult) {
                const resultDisplay = document.getElementById('apiContent');
                const specialCharsRegex = /[#*@\$]/g;
                const sanitizedResult = actionResult.replace(specialCharsRegex, '');
                resultDisplay.textContent = `${sanitizedResult}`;
                resultDisplay.style.display = 'block';
                resultDisplay.style.whiteSpace = 'pre-wrap';
                switch (currentAction) {
                    case 'compress':
                        currentAction = 'compress';
                        document.getElementById('mainButton').textContent = '压缩文件';
                        break;
                    case 'decompress':
                        currentAction = 'decompress';
                        document.getElementById('mainButton').textContent = '解压缩文件';
                        break;
                    case 'scan':
                        currentAction = 'scan';
                        document.getElementById('mainButton').textContent = '查杀文件';
                        break;
                    case 'clean':
                        currentAction = 'clean';
                        document.getElementById('mainButton').textContent = '清理文件';
                        break;
                    case 'git_load':
                        currentAction = 'git_load';
                        document.getElementById('mainButton').textContent = 'git上传文件';
                        break;
                    default:
                        alert('无法识别的操作');
                        return;
                }
                submitFormLocal();
            } else {
                alert('API返回错误或未识别操作');
                return;
            }
        } catch (error) {
            console.error('Error during API call:', error);
            alert('请求错误，请稍后重试');
        }
    } else {
        submitFormLocal();
    }
}

async function submitFormLocal() {
    document.getElementById('currentAction').value = currentAction;
    document.getElementById('pathHidden').value = document.getElementById('path').value;
    alert(`执行操作: ${currentAction}`);
    const formData = new FormData(document.querySelector('form'));

    try {
        const response = await fetch('/Home/UploadFile', {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            const result = await response.json();
            if (result.showFileUpload) {
                document.getElementById('localContent').innerHTML = result.folderInfo;
            } else {
                alert('操作失败: ' + result.message);
            }
        } else {
            console.error('Server error:', response.status);
            alert('上传失败，请稍后重试');
        }

    } catch (error) {
        console.error('Network error:', error);
        alert('请求错误，请稍后重试');
    }
}

function toggleAction() {
    if (currentAction === 'scan') {
        currentAction = 'clean';
        document.getElementById('mainButton').textContent = '清理文件';
    } else if (currentAction === 'clean') {
        currentAction = 'compress';
        document.getElementById('mainButton').textContent = '压缩文件';
    } else if (currentAction === 'compress') {
        currentAction = 'git_load';
        document.getElementById('mainButton').textContent = 'git上传文件';
    } else if (currentAction === 'git_load') {
        currentAction = 'decompress';
        document.getElementById('mainButton').textContent = '解压缩文件';
    } else if (currentAction === 'decompress') {
        currentAction = 'scan';
        document.getElementById('mainButton').textContent = '查杀文件';
    }
}

function checkFileType() {
    var fileInput = document.getElementById("file");
    var folderInput = document.getElementById("folder");
    var warningMessage = document.getElementById("warningMessage");

    if (fileInput.files.length > 0) {
        folderInput.disabled = true;
        warningMessage.style.display = "block";
        warningMessage.innerHTML = "您已选择文件，不能选择文件夹进行上传。";
    } else {
        folderInput.disabled = false;
    }
    if (folderInput.files.length > 0) {
        fileInput.disabled = true;
        warningMessage.style.display = "block";
        warningMessage.innerHTML = "您已选择文件夹，不能选择文件进行上传。";
    } else {
        fileInput.disabled = false;
    }
}

// NLP API 调用请通过后端 Controller 代理（server-side），不要在前端暴露 API key
// 如需使用，请在 HomeController 中新增端点调用第三方 API，前端改为 fetch('/Home/CallNLPApi', ...)
// async function callNLPApi(actionDescription) { ... }
