import { configureUmbCheckoutClient as _ } from "@umbcheckout/core";
const G = {
  bodySerializer: (t) => JSON.stringify(
    t,
    (e, r) => typeof r == "bigint" ? r.toString() : r
  )
}, M = ({
  onRequest: t,
  onSseError: e,
  onSseEvent: r,
  responseTransformer: o,
  responseValidator: n,
  sseDefaultRetryDelay: l,
  sseMaxRetryAttempts: c,
  sseMaxRetryDelay: a,
  sseSleepFn: i,
  url: u,
  ...s
}) => {
  let d;
  const z = i ?? ((f) => new Promise((y) => setTimeout(y, f)));
  return { stream: async function* () {
    let f = l ?? 3e3, y = 0;
    const S = s.signal ?? new AbortController().signal;
    for (; !S.aborted; ) {
      y++;
      const x = s.headers instanceof Headers ? s.headers : new Headers(s.headers);
      d !== void 0 && x.set("Last-Event-ID", d);
      try {
        const j = {
          redirect: "follow",
          ...s,
          body: s.serializedBody,
          headers: x,
          signal: S
        };
        let m = new Request(u, j);
        t && (m = await t(u, j));
        const p = await (s.fetch ?? globalThis.fetch)(m);
        if (!p.ok)
          throw new Error(
            `SSE failed: ${p.status} ${p.statusText}`
          );
        if (!p.body) throw new Error("No body in SSE response");
        const w = p.body.pipeThrough(new TextDecoderStream()).getReader();
        let k = "";
        const T = () => {
          try {
            w.cancel();
          } catch {
          }
        };
        S.addEventListener("abort", T);
        try {
          for (; ; ) {
            const { done: V, value: L } = await w.read();
            if (V) break;
            k += L;
            const $ = k.split(`

`);
            k = $.pop() ?? "";
            for (const J of $) {
              const F = J.split(`
`), q = [];
              let I;
              for (const b of F)
                if (b.startsWith("data:"))
                  q.push(b.replace(/^data:\s*/, ""));
                else if (b.startsWith("event:"))
                  I = b.replace(/^event:\s*/, "");
                else if (b.startsWith("id:"))
                  d = b.replace(/^id:\s*/, "");
                else if (b.startsWith("retry:")) {
                  const U = Number.parseInt(
                    b.replace(/^retry:\s*/, ""),
                    10
                  );
                  Number.isNaN(U) || (f = U);
                }
              let E, B = !1;
              if (q.length) {
                const b = q.join(`
`);
                try {
                  E = JSON.parse(b), B = !0;
                } catch {
                  E = b;
                }
              }
              B && (n && await n(E), o && (E = await o(E))), r?.({
                data: E,
                event: I,
                id: d,
                retry: f
              }), q.length && (yield E);
            }
          }
        } finally {
          S.removeEventListener("abort", T), w.releaseLock();
        }
        break;
      } catch (j) {
        if (e?.(j), c !== void 0 && y >= c)
          break;
        const m = Math.min(
          f * 2 ** (y - 1),
          a ?? 3e4
        );
        await z(m);
      }
    }
  }() };
}, Q = (t) => {
  switch (t) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, K = (t) => {
  switch (t) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, X = (t) => {
  switch (t) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, P = ({
  allowReserved: t,
  explode: e,
  name: r,
  style: o,
  value: n
}) => {
  if (!e) {
    const a = (t ? n : n.map((i) => encodeURIComponent(i))).join(K(o));
    switch (o) {
      case "label":
        return `.${a}`;
      case "matrix":
        return `;${r}=${a}`;
      case "simple":
        return a;
      default:
        return `${r}=${a}`;
    }
  }
  const l = Q(o), c = n.map((a) => o === "label" || o === "simple" ? t ? a : encodeURIComponent(a) : O({
    allowReserved: t,
    name: r,
    value: a
  })).join(l);
  return o === "label" || o === "matrix" ? l + c : c;
}, O = ({
  allowReserved: t,
  name: e,
  value: r
}) => {
  if (r == null)
    return "";
  if (typeof r == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${e}=${t ? r : encodeURIComponent(r)}`;
}, v = ({
  allowReserved: t,
  explode: e,
  name: r,
  style: o,
  value: n,
  valueOnly: l
}) => {
  if (n instanceof Date)
    return l ? n.toISOString() : `${r}=${n.toISOString()}`;
  if (o !== "deepObject" && !e) {
    let i = [];
    Object.entries(n).forEach(([s, d]) => {
      i = [
        ...i,
        s,
        t ? d : encodeURIComponent(d)
      ];
    });
    const u = i.join(",");
    switch (o) {
      case "form":
        return `${r}=${u}`;
      case "label":
        return `.${u}`;
      case "matrix":
        return `;${r}=${u}`;
      default:
        return u;
    }
  }
  const c = X(o), a = Object.entries(n).map(
    ([i, u]) => O({
      allowReserved: t,
      name: o === "deepObject" ? `${r}[${i}]` : i,
      value: u
    })
  ).join(c);
  return o === "label" || o === "matrix" ? c + a : a;
}, Y = /\{[^{}]+\}/g, Z = ({ path: t, url: e }) => {
  let r = e;
  const o = e.match(Y);
  if (o)
    for (const n of o) {
      let l = !1, c = n.substring(1, n.length - 1), a = "simple";
      c.endsWith("*") && (l = !0, c = c.substring(0, c.length - 1)), c.startsWith(".") ? (c = c.substring(1), a = "label") : c.startsWith(";") && (c = c.substring(1), a = "matrix");
      const i = t[c];
      if (i == null)
        continue;
      if (Array.isArray(i)) {
        r = r.replace(
          n,
          P({ explode: l, name: c, style: a, value: i })
        );
        continue;
      }
      if (typeof i == "object") {
        r = r.replace(
          n,
          v({
            explode: l,
            name: c,
            style: a,
            value: i,
            valueOnly: !0
          })
        );
        continue;
      }
      if (a === "matrix") {
        r = r.replace(
          n,
          `;${O({
            name: c,
            value: i
          })}`
        );
        continue;
      }
      const u = encodeURIComponent(
        a === "label" ? `.${i}` : i
      );
      r = r.replace(n, u);
    }
  return r;
}, ee = ({
  baseUrl: t,
  path: e,
  query: r,
  querySerializer: o,
  url: n
}) => {
  const l = n.startsWith("/") ? n : `/${n}`;
  let c = (t ?? "") + l;
  e && (c = Z({ path: e, url: c }));
  let a = r ? o(r) : "";
  return a.startsWith("?") && (a = a.substring(1)), a && (c += `?${a}`), c;
};
function te(t) {
  const e = t.body !== void 0;
  if (e && t.bodySerializer)
    return "serializedBody" in t ? t.serializedBody !== void 0 && t.serializedBody !== "" ? t.serializedBody : null : t.body !== "" ? t.body : null;
  if (e)
    return t.body;
}
const re = async (t, e) => {
  const r = typeof e == "function" ? await e(t) : e;
  if (r)
    return t.scheme === "bearer" ? `Bearer ${r}` : t.scheme === "basic" ? `Basic ${btoa(r)}` : r;
}, W = ({
  allowReserved: t,
  array: e,
  object: r
} = {}) => (n) => {
  const l = [];
  if (n && typeof n == "object")
    for (const c in n) {
      const a = n[c];
      if (a != null)
        if (Array.isArray(a)) {
          const i = P({
            allowReserved: t,
            explode: !0,
            name: c,
            style: "form",
            value: a,
            ...e
          });
          i && l.push(i);
        } else if (typeof a == "object") {
          const i = v({
            allowReserved: t,
            explode: !0,
            name: c,
            style: "deepObject",
            value: a,
            ...r
          });
          i && l.push(i);
        } else {
          const i = O({
            allowReserved: t,
            name: c,
            value: a
          });
          i && l.push(i);
        }
    }
  return l.join("&");
}, se = (t) => {
  if (!t)
    return "stream";
  const e = t.split(";")[0]?.trim();
  if (e) {
    if (e.startsWith("application/json") || e.endsWith("+json"))
      return "json";
    if (e === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some(
      (r) => e.startsWith(r)
    ))
      return "blob";
    if (e.startsWith("text/"))
      return "text";
  }
}, ae = (t, e) => e ? !!(t.headers.has(e) || t.query?.[e] || t.headers.get("Cookie")?.includes(`${e}=`)) : !1, ne = async ({
  security: t,
  ...e
}) => {
  for (const r of t) {
    if (ae(e, r.name))
      continue;
    const o = await re(r, e.auth);
    if (!o)
      continue;
    const n = r.name ?? "Authorization";
    switch (r.in) {
      case "query":
        e.query || (e.query = {}), e.query[n] = o;
        break;
      case "cookie":
        e.headers.append("Cookie", `${n}=${o}`);
        break;
      default:
        e.headers.set(n, o);
        break;
    }
  }
}, D = (t) => ee({
  baseUrl: t.baseUrl,
  path: t.path,
  query: t.query,
  querySerializer: typeof t.querySerializer == "function" ? t.querySerializer : W(t.querySerializer),
  url: t.url
}), N = (t, e) => {
  const r = { ...t, ...e };
  return r.baseUrl?.endsWith("/") && (r.baseUrl = r.baseUrl.substring(0, r.baseUrl.length - 1)), r.headers = H(t.headers, e.headers), r;
}, ie = (t) => {
  const e = [];
  return t.forEach((r, o) => {
    e.push([o, r]);
  }), e;
}, H = (...t) => {
  const e = new Headers();
  for (const r of t) {
    if (!r)
      continue;
    const o = r instanceof Headers ? ie(r) : Object.entries(r);
    for (const [n, l] of o)
      if (l === null)
        e.delete(n);
      else if (Array.isArray(l))
        for (const c of l)
          e.append(n, c);
      else l !== void 0 && e.set(
        n,
        typeof l == "object" ? JSON.stringify(l) : l
      );
  }
  return e;
};
class A {
  constructor() {
    this.fns = [];
  }
  clear() {
    this.fns = [];
  }
  eject(e) {
    const r = this.getInterceptorIndex(e);
    this.fns[r] && (this.fns[r] = null);
  }
  exists(e) {
    const r = this.getInterceptorIndex(e);
    return !!this.fns[r];
  }
  getInterceptorIndex(e) {
    return typeof e == "number" ? this.fns[e] ? e : -1 : this.fns.indexOf(e);
  }
  update(e, r) {
    const o = this.getInterceptorIndex(e);
    return this.fns[o] ? (this.fns[o] = r, e) : !1;
  }
  use(e) {
    return this.fns.push(e), this.fns.length - 1;
  }
}
const oe = () => ({
  error: new A(),
  request: new A(),
  response: new A()
}), ce = W({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), le = {
  "Content-Type": "application/json"
}, R = (t = {}) => ({
  ...G,
  headers: le,
  parseAs: "auto",
  querySerializer: ce,
  ...t
}), fe = (t = {}) => {
  let e = N(R(), t);
  const r = () => ({ ...e }), o = (u) => (e = N(e, u), r()), n = oe(), l = async (u) => {
    const s = {
      ...e,
      ...u,
      fetch: u.fetch ?? e.fetch ?? globalThis.fetch,
      headers: H(e.headers, u.headers),
      serializedBody: void 0
    };
    s.security && await ne({
      ...s,
      security: s.security
    }), s.requestValidator && await s.requestValidator(s), s.body !== void 0 && s.bodySerializer && (s.serializedBody = s.bodySerializer(s.body)), (s.body === void 0 || s.serializedBody === "") && s.headers.delete("Content-Type");
    const d = D(s);
    return { opts: s, url: d };
  }, c = async (u) => {
    const { opts: s, url: d } = await l(u), z = {
      redirect: "follow",
      ...s,
      body: te(s)
    };
    let g = new Request(d, z);
    for (const h of n.request.fns)
      h && (g = await h(g, s));
    const C = s.fetch;
    let f = await C(g);
    for (const h of n.response.fns)
      h && (f = await h(f, g, s));
    const y = {
      request: g,
      response: f
    };
    if (f.ok) {
      const h = (s.parseAs === "auto" ? se(f.headers.get("Content-Type")) : s.parseAs) ?? "json";
      if (f.status === 204 || f.headers.get("Content-Length") === "0") {
        let w;
        switch (h) {
          case "arrayBuffer":
          case "blob":
          case "text":
            w = await f[h]();
            break;
          case "formData":
            w = new FormData();
            break;
          case "stream":
            w = f.body;
            break;
          default:
            w = {};
            break;
        }
        return s.responseStyle === "data" ? w : {
          data: w,
          ...y
        };
      }
      let p;
      switch (h) {
        case "arrayBuffer":
        case "blob":
        case "formData":
        case "json":
        case "text":
          p = await f[h]();
          break;
        case "stream":
          return s.responseStyle === "data" ? f.body : {
            data: f.body,
            ...y
          };
      }
      return h === "json" && (s.responseValidator && await s.responseValidator(p), s.responseTransformer && (p = await s.responseTransformer(p))), s.responseStyle === "data" ? p : {
        data: p,
        ...y
      };
    }
    const S = await f.text();
    let x;
    try {
      x = JSON.parse(S);
    } catch {
    }
    const j = x ?? S;
    let m = j;
    for (const h of n.error.fns)
      h && (m = await h(j, f, g, s));
    if (m = m || {}, s.throwOnError)
      throw m;
    return s.responseStyle === "data" ? void 0 : {
      error: m,
      ...y
    };
  }, a = (u) => (s) => c({ ...s, method: u }), i = (u) => async (s) => {
    const { opts: d, url: z } = await l(s);
    return M({
      ...d,
      body: d.body,
      headers: d.headers,
      method: u,
      onRequest: async (g, C) => {
        let f = new Request(g, C);
        for (const y of n.request.fns)
          y && (f = await y(f, d));
        return f;
      },
      url: z
    });
  };
  return {
    buildUrl: D,
    connect: a("CONNECT"),
    delete: a("DELETE"),
    get: a("GET"),
    getConfig: r,
    head: a("HEAD"),
    interceptors: n,
    options: a("OPTIONS"),
    patch: a("PATCH"),
    post: a("POST"),
    put: a("PUT"),
    request: c,
    setConfig: o,
    sse: {
      connect: i("CONNECT"),
      delete: i("DELETE"),
      get: i("GET"),
      head: i("HEAD"),
      options: i("OPTIONS"),
      patch: i("PATCH"),
      post: i("POST"),
      put: i("PUT"),
      trace: i("TRACE")
    },
    trace: a("TRACE")
  };
}, ue = fe(R({
  baseUrl: "https://localhost:44380",
  throwOnError: !0
})), he = (t) => {
  _(t, ue);
}, ye = () => {
};
export {
  he as onInit,
  ye as onUnload
};
//# sourceMappingURL=entrypoint-B7Nx1T05.js.map
