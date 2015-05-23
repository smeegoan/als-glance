als_glance.ApiClient = function (config) {
    var authToken = config.authToken,
        baseUri = config.baseUri,
        configureRequest = function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + authToken);
        };

    this.createUri = function (path) {
        return baseUri + path;
    };

    this.get = function (path, query) {
        return $.ajax({
            url: this.createUri(path),
            type: "GET",
            beforeSend: configureRequest
        });
    };

    this.post = function (path, data) {
        return $.ajax({
            url: this.createUri(path),
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: data,
            beforeSend: configureRequest
        });
    };

    this.put = function (path, data) {
        return $.ajax({
            url: this.createUri(path),
            type: "PUT",
            contentType: "application/json",
            dataType: "json",
            data: data,
            beforeSend: configureRequest
        });
    };

    this.delete = function(path) {
        return $.ajax({
            url: this.createUri(path),
            type: "DELETE",
            dataType: "json",
            beforeSend: configureRequest
        });
    };
};

var helper = function (data) {
    var result = {};

    function recurse(cur, prop) {
        if (Object(cur) !== cur) {
            result[prop] = cur;
        } else if (Array.isArray(cur)) {
            for (var i = 0, l = cur.length; i < l; i++)
                recurse(cur[i], prop);
            if (l == 0) result[prop] = [];
        } else {
            var isEmpty = true;
            for (var p in cur) {
                isEmpty = false;
                //if (p != "Id")
                recurse(cur[p], prop ? prop + p : p);
            }
            if (isEmpty && prop)
                result[prop] = {};
        }
    }
    recurse(data, "");
    return result;
};

function groupBy(items, propertyName) {
    var result = [];
    $.each(items, function (index, item) {
        if ($.inArray(item[propertyName], result) == -1) {
            result.push(item[propertyName]);
        }
    });
    return result;
}

JSON.flatten = function (data) {
    var res = [];
    for (var p in data) {
        if (Array.isArray(data[p])) {
            for (var value in data[p]) {
                res.push(helper(data[p][value]));
            }
        }
    }
   //review this res.pop(); // the last element is an empty object
    return res;
};


JSON.unflatten = function (data) {
    if (Object(data) !== data || Array.isArray(data)) return data;
    var regex = /\.?([^.\[\]]+)|\[(\d+)\]/g,
        resultholder = {};
    for (var p in data) {
        var cur = resultholder,
            prop = "",
            m;
        while (m = regex.exec(p)) {
            cur = cur[prop] || (cur[prop] = (m[2] ? [] : {}));
            prop = m[2] || m[1];
        }
        cur[prop] = data[p];
    }
    return resultholder[""] || resultholder;
};
