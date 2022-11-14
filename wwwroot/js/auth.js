let auth_block = document.getElementById("auth_block");
    auth_block.innerHTML = `
                <form style="display: block; position: relative" id="login" action="/Main/Index" method="post" align="center">
                    <h2>Ваш адрес кошелька:</h2>
                    <input type="text" placeholder="Введите адрес кошелька..." id="Address" name="vm.address" />
                    <h2>Ваш публичный ключ:</h2>
                    <input type="text" placeholder="Введите публичный ключ..." id="public_key" name="vm.public_key"/>
                    <h2>Ваш секретный ключ:</h2>
                    <input type="text" placeholder="Введите секретный ключ..." id="secret_key" name="vm.secret_key"/>
                    <br><br>
                    <input type="submit" class="buttonscript button" value="Авторизоваться" id="auth"/>
                    <br>
                    <hr>
                    <h2>У Вас нет учётной записи? <a style="cursor: pointer; text-decoration-line: underline; color: darkcyan;" id="toggle">Зарегистрируйтесь</a></h2>
                </form>
                <form style="display: none" id="register" action="/Main/Index" method="post">
                <input type="text" placeholder="Введите публичный ключ..." id="public_keyR" name="vm.public_key" style = "visibility: hidden; display: none"/>
                <input type="password" placeholder="Введите секретный ключ..." id="secret_keyR" name="vm.secret_key" style = "visibility: hidden; display: none"/>
                <input type="text" placeholder="Введите публичный ключ..." id="AddressR" name="vm.address" style = "visibility: hidden; display: none"/>
                    <input type="submit" class="buttonscript button" value="Зарегистрироваться" id="reg"/>
                    <br>
                    <hr>
                    <h2>У Вас есть учётная запись? <a style="cursor: pointer; text-decoration-line: underline; color: darkcyan;" id="toggle">Авторизуйтесь</a></h2>
                </form>`;

    let login = document.getElementById("login");
    let register = document.getElementById("register");

    if (item != null) {
        pkey = JSON.parse(item).public_key;
        skey = JSON.parse(item).secret_key;
        address = JSON.parse(item).address;
        var req = new XMLHttpRequest();
    req.open("POST", "/Main/Auth?vm.public_key=" + pkey + "&vm.secret_key=" + skey + "&vm.address=" + address, false);
    req.send(null);
        if (req.responseText == "True") auth_block.remove();
        document.getElementById("index").href += "?vm.public_key=" + pkey + "&vm.secret_key=" + skey + "&vm.address=" + address;
        document.getElementById("pers_area").href += "?vm.public_key=" + pkey + "&vm.secret_key=" + skey + "&vm.address=" + address;
        document.getElementById("chat").href += "?vm.public_key=" + pkey + "&vm.secret_key=" + skey + "&vm.address=" + address;
    }
    document.addEventListener("click", auth_function = (e) => {
        if(e.target.id == "toggle") {
            if(login.style.display === "block") {
                login.style.display = "none";
                register.style.display = "block";
            } else {
                login.style.display = "block";
                register.style.display = "none";
            }
        }
        if (e.target.id == "auth") {
            let key = {
                public_key: document.getElementById("public_key").value,
                secret_key: document.getElementById("secret_key").value,
                address: document.getElementById("Address").value
            }
            req = new XMLHttpRequest();
            req.open("POST", "/Main/Auth?vm.public_key=" + key.public_key + "&vm.secret_key=" + key.secret_key + "&vm.address=" + key.address, false);
            req.send(null);
            if (req.responseText == "True") window.localStorage.setItem("PersonLog", JSON.stringify(key));
        }
        if (e.target.id == "reg") {
            req = new XMLHttpRequest();
            req.open("POST", "/Main/Reg", false);
            req.send(null);
            if (req.responseText != "") {
                Storage.setItem("PersonLog", req.responseText);
            }
        }
    });
    