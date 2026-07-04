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

async function callNLPApi(actionDescription) {
    const url = "https://api.siliconflow.cn/v1/chat/completions";
    const token = ""; // 请在环境变量或后端配置中设置 SiliconFlow API Key

    const requestData = {
        model: "deepseek-ai/DeepSeek-V2.5",
        messages: [
            {role: "user", content: actionDescription}
        ],
        stream: false,
        max_tokens: 512,
        stop: ["null"],
        temperature: 0.8,
        top_p: 0.8,
        top_k: 75,
        frequency_penalty: 0.5,
        n: 2,
        response_format: {type: "text"},
    };

    const options = {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestData)
    };

    try {
        const response = await fetch(url, options);
        const data = await response.json();

        if (response.ok) {
            return data.choices && data.choices.length > 0 ? data.choices[0].message.content : null;
        } else {
            console.error('API Error:', data);
            return null;
        }
    } catch (error) {
        console.error('Network Error:', error);
        return null;
    }
}
