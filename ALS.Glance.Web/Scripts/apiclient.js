﻿'use strict';
var alsglance = alsglance || {};
alsglance.ApiClientImpl = function (config) {
    var authToken = config.authToken,
        baseUri = config.baseUri,
        configureRequest = function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + authToken);
        },
        configureETagRequest = function (etag) {
            return function(xhr) {
                configureRequest(xhr);
                xhr.setRequestHeader("If-Match", etag);
            };
        };

    this.createUri = function (path) {
        return baseUri + path;
    };

    this.get = function (path, query) {
        return $.ajax({
            url: this.createUri(path),
            type: "GET",
            beforeSend: configureRequest,
            error: function (err) {
                toastr.error(err.statusText, 'ALS Glance');
            }
        });
    };

    this.post = function (path, data) {

        return $.ajax({
            url: this.createUri(path),
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: data,
            beforeSend: configureRequest,
            error: function (err) {
                toastr.error(err.statusText, 'ALS Glance');
            }
        });
    };

    this.put = function (path, data, etag) {
        return $.ajax({
            url: this.createUri(path),
            type: "PUT",
            contentType: "application/json",
            dataType: "json",
            data: data,
            beforeSend: etag != null ? configureETagRequest(etag) : configureRequest,
            error: function (err) {
                toastr.error(err.statusText, 'ALS Glance');
            }
        });
    };

    this.delete = function (path) {
        return $.ajax({
            url: this.createUri(path),
            type: "DELETE",
            dataType: "json",
            beforeSend: configureRequest,
            error: function (err) {
                toastr.error(err.statusText, 'ALS Glance');
            }
        });
    };
};

//var helper = function (data) {
//    var result = {};

//    function recurse(cur, prop) {
//        if (Object(cur) !== cur) {
//            result[prop] = cur;
//        } else if (Array.isArray(cur)) {
//            for (var i = 0, l = cur.length; i < l; i++)
//                recurse(cur[i], prop);
//            if (l == 0) result[prop] = [];
//        } else {
//            var isEmpty = true;
//            for (var p in cur) {
//                isEmpty = false;
//                //if (p != "Id")
//                recurse(cur[p], prop ? prop + p : p);
//            }
//            if (isEmpty && prop)
//                result[prop] = {};
//        }
//    }
//    recurse(data, "");
//    return result;
//};

//JSON.flatten = function (data) {
//    var res = [];
//    for (var p in data) {
//        if (Array.isArray(data[p])) {
//            for (var value in data[p]) {
//                res.push(helper(data[p][value]));
//            }
//        }
//    }
//   //review this res.pop(); // the last element is an empty object
//    return res;
//};


//JSON.unflatten = function (data) {
//    if (Object(data) !== data || Array.isArray(data)) return data;
//    var regex = /\.?([^.\[\]]+)|\[(\d+)\]/g,
//        resultholder = {};
//    for (var p in data) {
//        var cur = resultholder,
//            prop = "",
//            m;
//        while (m = regex.exec(p)) {
//            cur = cur[prop] || (cur[prop] = (m[2] ? [] : {}));
//            prop = m[2] || m[1];
//        }
//        cur[prop] = data[p];
//    }
//    return resultholder[""] || resultholder;
//};
