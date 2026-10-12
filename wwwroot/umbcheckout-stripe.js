/**
 * @license
 * Copyright 2019 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const I = globalThis, F = I.ShadowRoot && (I.ShadyCSS === void 0 || I.ShadyCSS.nativeShadow) && "adoptedStyleSheets" in Document.prototype && "replace" in CSSStyleSheet.prototype, G = Symbol(), ie = /* @__PURE__ */ new WeakMap();
let _e = class {
  constructor(e, t, r) {
    if (this._$cssResult$ = !0, r !== G) throw Error("CSSResult is not constructable. Use `unsafeCSS` or `css` instead.");
    this.cssText = e, this.t = t;
  }
  get styleSheet() {
    let e = this.o;
    const t = this.t;
    if (F && e === void 0) {
      const r = t !== void 0 && t.length === 1;
      r && (e = ie.get(t)), e === void 0 && ((this.o = e = new CSSStyleSheet()).replaceSync(this.cssText), r && ie.set(t, e));
    }
    return e;
  }
  toString() {
    return this.cssText;
  }
};
const ge = (s) => new _e(typeof s == "string" ? s : s + "", void 0, G), Y = (s, ...e) => {
  const t = s.length === 1 ? s[0] : e.reduce((r, i, a) => r + ((o) => {
    if (o._$cssResult$ === !0) return o.cssText;
    if (typeof o == "number") return o;
    throw Error("Value passed to 'css' function must be a 'css' function result: " + o + ". Use 'unsafeCSS' to pass non-literal values, but take care to ensure page security.");
  })(i) + s[a + 1], s[0]);
  return new _e(t, s, G);
}, Se = (s, e) => {
  if (F) s.adoptedStyleSheets = e.map((t) => t instanceof CSSStyleSheet ? t : t.styleSheet);
  else for (const t of e) {
    const r = document.createElement("style"), i = I.litNonce;
    i !== void 0 && r.setAttribute("nonce", i), r.textContent = t.cssText, s.appendChild(r);
  }
}, se = F ? (s) => s : (s) => s instanceof CSSStyleSheet ? ((e) => {
  let t = "";
  for (const r of e.cssRules) t += r.cssText;
  return ge(t);
})(s) : s;
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const { is: Ae, defineProperty: we, getOwnPropertyDescriptor: Ee, getOwnPropertyNames: Ce, getOwnPropertySymbols: xe, getPrototypeOf: Pe } = Object, m = globalThis, re = m.trustedTypes, Re = re ? re.emptyScript : "", V = m.reactiveElementPolyfillSupport, T = (s, e) => s, L = { toAttribute(s, e) {
  switch (e) {
    case Boolean:
      s = s ? Re : null;
      break;
    case Object:
    case Array:
      s = s == null ? s : JSON.stringify(s);
  }
  return s;
}, fromAttribute(s, e) {
  let t = s;
  switch (e) {
    case Boolean:
      t = s !== null;
      break;
    case Number:
      t = s === null ? null : Number(s);
      break;
    case Object:
    case Array:
      try {
        t = JSON.parse(s);
      } catch {
        t = null;
      }
  }
  return t;
} }, Q = (s, e) => !Ae(s, e), ae = { attribute: !0, type: String, converter: L, reflect: !1, useDefault: !1, hasChanged: Q };
Symbol.metadata ?? (Symbol.metadata = Symbol("metadata")), m.litPropertyMetadata ?? (m.litPropertyMetadata = /* @__PURE__ */ new WeakMap());
let x = class extends HTMLElement {
  static addInitializer(e) {
    this._$Ei(), (this.l ?? (this.l = [])).push(e);
  }
  static get observedAttributes() {
    return this.finalize(), this._$Eh && [...this._$Eh.keys()];
  }
  static createProperty(e, t = ae) {
    if (t.state && (t.attribute = !1), this._$Ei(), this.prototype.hasOwnProperty(e) && ((t = Object.create(t)).wrapped = !0), this.elementProperties.set(e, t), !t.noAccessor) {
      const r = Symbol(), i = this.getPropertyDescriptor(e, r, t);
      i !== void 0 && we(this.prototype, e, i);
    }
  }
  static getPropertyDescriptor(e, t, r) {
    const { get: i, set: a } = Ee(this.prototype, e) ?? { get() {
      return this[t];
    }, set(o) {
      this[t] = o;
    } };
    return { get: i, set(o) {
      const l = i == null ? void 0 : i.call(this);
      a == null || a.call(this, o), this.requestUpdate(e, l, r);
    }, configurable: !0, enumerable: !0 };
  }
  static getPropertyOptions(e) {
    return this.elementProperties.get(e) ?? ae;
  }
  static _$Ei() {
    if (this.hasOwnProperty(T("elementProperties"))) return;
    const e = Pe(this);
    e.finalize(), e.l !== void 0 && (this.l = [...e.l]), this.elementProperties = new Map(e.elementProperties);
  }
  static finalize() {
    if (this.hasOwnProperty(T("finalized"))) return;
    if (this.finalized = !0, this._$Ei(), this.hasOwnProperty(T("properties"))) {
      const t = this.properties, r = [...Ce(t), ...xe(t)];
      for (const i of r) this.createProperty(i, t[i]);
    }
    const e = this[Symbol.metadata];
    if (e !== null) {
      const t = litPropertyMetadata.get(e);
      if (t !== void 0) for (const [r, i] of t) this.elementProperties.set(r, i);
    }
    this._$Eh = /* @__PURE__ */ new Map();
    for (const [t, r] of this.elementProperties) {
      const i = this._$Eu(t, r);
      i !== void 0 && this._$Eh.set(i, t);
    }
    this.elementStyles = this.finalizeStyles(this.styles);
  }
  static finalizeStyles(e) {
    const t = [];
    if (Array.isArray(e)) {
      const r = new Set(e.flat(1 / 0).reverse());
      for (const i of r) t.unshift(se(i));
    } else e !== void 0 && t.push(se(e));
    return t;
  }
  static _$Eu(e, t) {
    const r = t.attribute;
    return r === !1 ? void 0 : typeof r == "string" ? r : typeof e == "string" ? e.toLowerCase() : void 0;
  }
  constructor() {
    super(), this._$Ep = void 0, this.isUpdatePending = !1, this.hasUpdated = !1, this._$Em = null, this._$Ev();
  }
  _$Ev() {
    var e;
    this._$ES = new Promise((t) => this.enableUpdating = t), this._$AL = /* @__PURE__ */ new Map(), this._$E_(), this.requestUpdate(), (e = this.constructor.l) == null || e.forEach((t) => t(this));
  }
  addController(e) {
    var t;
    (this._$EO ?? (this._$EO = /* @__PURE__ */ new Set())).add(e), this.renderRoot !== void 0 && this.isConnected && ((t = e.hostConnected) == null || t.call(e));
  }
  removeController(e) {
    var t;
    (t = this._$EO) == null || t.delete(e);
  }
  _$E_() {
    const e = /* @__PURE__ */ new Map(), t = this.constructor.elementProperties;
    for (const r of t.keys()) this.hasOwnProperty(r) && (e.set(r, this[r]), delete this[r]);
    e.size > 0 && (this._$Ep = e);
  }
  createRenderRoot() {
    const e = this.shadowRoot ?? this.attachShadow(this.constructor.shadowRootOptions);
    return Se(e, this.constructor.elementStyles), e;
  }
  connectedCallback() {
    var e;
    this.renderRoot ?? (this.renderRoot = this.createRenderRoot()), this.enableUpdating(!0), (e = this._$EO) == null || e.forEach((t) => {
      var r;
      return (r = t.hostConnected) == null ? void 0 : r.call(t);
    });
  }
  enableUpdating(e) {
  }
  disconnectedCallback() {
    var e;
    (e = this._$EO) == null || e.forEach((t) => {
      var r;
      return (r = t.hostDisconnected) == null ? void 0 : r.call(t);
    });
  }
  attributeChangedCallback(e, t, r) {
    this._$AK(e, r);
  }
  _$ET(e, t) {
    var a;
    const r = this.constructor.elementProperties.get(e), i = this.constructor._$Eu(e, r);
    if (i !== void 0 && r.reflect === !0) {
      const o = (((a = r.converter) == null ? void 0 : a.toAttribute) !== void 0 ? r.converter : L).toAttribute(t, r.type);
      this._$Em = e, o == null ? this.removeAttribute(i) : this.setAttribute(i, o), this._$Em = null;
    }
  }
  _$AK(e, t) {
    var a, o;
    const r = this.constructor, i = r._$Eh.get(e);
    if (i !== void 0 && this._$Em !== i) {
      const l = r.getPropertyOptions(i), n = typeof l.converter == "function" ? { fromAttribute: l.converter } : ((a = l.converter) == null ? void 0 : a.fromAttribute) !== void 0 ? l.converter : L;
      this._$Em = i;
      const u = n.fromAttribute(t, l.type);
      this[i] = u ?? ((o = this._$Ej) == null ? void 0 : o.get(i)) ?? u, this._$Em = null;
    }
  }
  requestUpdate(e, t, r, i = !1, a) {
    var o;
    if (e !== void 0) {
      const l = this.constructor;
      if (i === !1 && (a = this[e]), r ?? (r = l.getPropertyOptions(e)), !((r.hasChanged ?? Q)(a, t) || r.useDefault && r.reflect && a === ((o = this._$Ej) == null ? void 0 : o.get(e)) && !this.hasAttribute(l._$Eu(e, r)))) return;
      this.C(e, t, r);
    }
    this.isUpdatePending === !1 && (this._$ES = this._$EP());
  }
  C(e, t, { useDefault: r, reflect: i, wrapped: a }, o) {
    r && !(this._$Ej ?? (this._$Ej = /* @__PURE__ */ new Map())).has(e) && (this._$Ej.set(e, o ?? t ?? this[e]), a !== !0 || o !== void 0) || (this._$AL.has(e) || (this.hasUpdated || r || (t = void 0), this._$AL.set(e, t)), i === !0 && this._$Em !== e && (this._$Eq ?? (this._$Eq = /* @__PURE__ */ new Set())).add(e));
  }
  async _$EP() {
    this.isUpdatePending = !0;
    try {
      await this._$ES;
    } catch (t) {
      Promise.reject(t);
    }
    const e = this.scheduleUpdate();
    return e != null && await e, !this.isUpdatePending;
  }
  scheduleUpdate() {
    return this.performUpdate();
  }
  performUpdate() {
    var r;
    if (!this.isUpdatePending) return;
    if (!this.hasUpdated) {
      if (this.renderRoot ?? (this.renderRoot = this.createRenderRoot()), this._$Ep) {
        for (const [a, o] of this._$Ep) this[a] = o;
        this._$Ep = void 0;
      }
      const i = this.constructor.elementProperties;
      if (i.size > 0) for (const [a, o] of i) {
        const { wrapped: l } = o, n = this[a];
        l !== !0 || this._$AL.has(a) || n === void 0 || this.C(a, void 0, o, n);
      }
    }
    let e = !1;
    const t = this._$AL;
    try {
      e = this.shouldUpdate(t), e ? (this.willUpdate(t), (r = this._$EO) == null || r.forEach((i) => {
        var a;
        return (a = i.hostUpdate) == null ? void 0 : a.call(i);
      }), this.update(t)) : this._$EM();
    } catch (i) {
      throw e = !1, this._$EM(), i;
    }
    e && this._$AE(t);
  }
  willUpdate(e) {
  }
  _$AE(e) {
    var t;
    (t = this._$EO) == null || t.forEach((r) => {
      var i;
      return (i = r.hostUpdated) == null ? void 0 : i.call(r);
    }), this.hasUpdated || (this.hasUpdated = !0, this.firstUpdated(e)), this.updated(e);
  }
  _$EM() {
    this._$AL = /* @__PURE__ */ new Map(), this.isUpdatePending = !1;
  }
  get updateComplete() {
    return this.getUpdateComplete();
  }
  getUpdateComplete() {
    return this._$ES;
  }
  shouldUpdate(e) {
    return !0;
  }
  update(e) {
    this._$Eq && (this._$Eq = this._$Eq.forEach((t) => this._$ET(t, this[t]))), this._$EM();
  }
  updated(e) {
  }
  firstUpdated(e) {
  }
};
x.elementStyles = [], x.shadowRootOptions = { mode: "open" }, x[T("elementProperties")] = /* @__PURE__ */ new Map(), x[T("finalized")] = /* @__PURE__ */ new Map(), V == null || V({ ReactiveElement: x }), (m.reactiveElementVersions ?? (m.reactiveElementVersions = [])).push("2.1.2");
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const k = globalThis, oe = (s) => s, B = k.trustedTypes, ne = B ? B.createPolicy("lit-html", { createHTML: (s) => s }) : void 0, $e = "$lit$", y = `lit$${Math.random().toFixed(9).slice(2)}$`, ve = "?" + y, Oe = `<${ve}>`, E = document, U = () => E.createComment(""), N = (s) => s === null || typeof s != "object" && typeof s != "function", X = Array.isArray, Te = (s) => X(s) || typeof (s == null ? void 0 : s[Symbol.iterator]) == "function", W = `[ 	
\f\r]`, O = /<(?:(!--|\/[^a-zA-Z])|(\/?[a-zA-Z][^>\s]*)|(\/?$))/g, le = /-->/g, he = />/g, g = RegExp(`>|${W}(?:([^\\s"'>=/]+)(${W}*=${W}*(?:[^ 	
\f\r"'\`<>=]|("|')|))|$)`, "g"), ue = /'/g, ce = /"/g, fe = /^(?:script|style|textarea|title)$/i, ke = (s) => (e, ...t) => ({ _$litType$: s, strings: e, values: t }), d = ke(1), P = Symbol.for("lit-noChange"), c = Symbol.for("lit-nothing"), pe = /* @__PURE__ */ new WeakMap(), S = E.createTreeWalker(E, 129);
function ye(s, e) {
  if (!X(s) || !s.hasOwnProperty("raw")) throw Error("invalid template strings array");
  return ne !== void 0 ? ne.createHTML(e) : e;
}
const Ue = (s, e) => {
  const t = s.length - 1, r = [];
  let i, a = e === 2 ? "<svg>" : e === 3 ? "<math>" : "", o = O;
  for (let l = 0; l < t; l++) {
    const n = s[l];
    let u, p, h = -1, _ = 0;
    for (; _ < n.length && (o.lastIndex = _, p = o.exec(n), p !== null); ) _ = o.lastIndex, o === O ? p[1] === "!--" ? o = le : p[1] !== void 0 ? o = he : p[2] !== void 0 ? (fe.test(p[2]) && (i = RegExp("</" + p[2], "g")), o = g) : p[3] !== void 0 && (o = g) : o === g ? p[0] === ">" ? (o = i ?? O, h = -1) : p[1] === void 0 ? h = -2 : (h = o.lastIndex - p[2].length, u = p[1], o = p[3] === void 0 ? g : p[3] === '"' ? ce : ue) : o === ce || o === ue ? o = g : o === le || o === he ? o = O : (o = g, i = void 0);
    const f = o === g && s[l + 1].startsWith("/>") ? " " : "";
    a += o === O ? n + Oe : h >= 0 ? (r.push(u), n.slice(0, h) + $e + n.slice(h) + y + f) : n + y + (h === -2 ? l : f);
  }
  return [ye(s, a + (s[t] || "<?>") + (e === 2 ? "</svg>" : e === 3 ? "</math>" : "")), r];
};
class H {
  constructor({ strings: e, _$litType$: t }, r) {
    let i;
    this.parts = [];
    let a = 0, o = 0;
    const l = e.length - 1, n = this.parts, [u, p] = Ue(e, t);
    if (this.el = H.createElement(u, r), S.currentNode = this.el.content, t === 2 || t === 3) {
      const h = this.el.content.firstChild;
      h.replaceWith(...h.childNodes);
    }
    for (; (i = S.nextNode()) !== null && n.length < l; ) {
      if (i.nodeType === 1) {
        if (i.hasAttributes()) for (const h of i.getAttributeNames()) if (h.endsWith($e)) {
          const _ = p[o++], f = i.getAttribute(h).split(y), j = /([.?@])?(.*)/.exec(_);
          n.push({ type: 1, index: a, name: j[2], strings: f, ctor: j[1] === "." ? He : j[1] === "?" ? Me : j[1] === "@" ? De : q }), i.removeAttribute(h);
        } else h.startsWith(y) && (n.push({ type: 6, index: a }), i.removeAttribute(h));
        if (fe.test(i.tagName)) {
          const h = i.textContent.split(y), _ = h.length - 1;
          if (_ > 0) {
            i.textContent = B ? B.emptyScript : "";
            for (let f = 0; f < _; f++) i.append(h[f], U()), S.nextNode(), n.push({ type: 2, index: ++a });
            i.append(h[_], U());
          }
        }
      } else if (i.nodeType === 8) if (i.data === ve) n.push({ type: 2, index: a });
      else {
        let h = -1;
        for (; (h = i.data.indexOf(y, h + 1)) !== -1; ) n.push({ type: 7, index: a }), h += y.length - 1;
      }
      a++;
    }
  }
  static createElement(e, t) {
    const r = E.createElement("template");
    return r.innerHTML = e, r;
  }
}
function R(s, e, t = s, r) {
  var o, l;
  if (e === P) return e;
  let i = r !== void 0 ? (o = t._$Co) == null ? void 0 : o[r] : t._$Cl;
  const a = N(e) ? void 0 : e._$litDirective$;
  return (i == null ? void 0 : i.constructor) !== a && ((l = i == null ? void 0 : i._$AO) == null || l.call(i, !1), a === void 0 ? i = void 0 : (i = new a(s), i._$AT(s, t, r)), r !== void 0 ? (t._$Co ?? (t._$Co = []))[r] = i : t._$Cl = i), i !== void 0 && (e = R(s, i._$AS(s, e.values), i, r)), e;
}
class Ne {
  constructor(e, t) {
    this._$AV = [], this._$AN = void 0, this._$AD = e, this._$AM = t;
  }
  get parentNode() {
    return this._$AM.parentNode;
  }
  get _$AU() {
    return this._$AM._$AU;
  }
  u(e) {
    const { el: { content: t }, parts: r } = this._$AD, i = ((e == null ? void 0 : e.creationScope) ?? E).importNode(t, !0);
    S.currentNode = i;
    let a = S.nextNode(), o = 0, l = 0, n = r[0];
    for (; n !== void 0; ) {
      if (o === n.index) {
        let u;
        n.type === 2 ? u = new D(a, a.nextSibling, this, e) : n.type === 1 ? u = new n.ctor(a, n.name, n.strings, this, e) : n.type === 6 && (u = new ze(a, this, e)), this._$AV.push(u), n = r[++l];
      }
      o !== (n == null ? void 0 : n.index) && (a = S.nextNode(), o++);
    }
    return S.currentNode = E, i;
  }
  p(e) {
    let t = 0;
    for (const r of this._$AV) r !== void 0 && (r.strings !== void 0 ? (r._$AI(e, r, t), t += r.strings.length - 2) : r._$AI(e[t])), t++;
  }
}
class D {
  get _$AU() {
    var e;
    return ((e = this._$AM) == null ? void 0 : e._$AU) ?? this._$Cv;
  }
  constructor(e, t, r, i) {
    this.type = 2, this._$AH = c, this._$AN = void 0, this._$AA = e, this._$AB = t, this._$AM = r, this.options = i, this._$Cv = (i == null ? void 0 : i.isConnected) ?? !0;
  }
  get parentNode() {
    let e = this._$AA.parentNode;
    const t = this._$AM;
    return t !== void 0 && (e == null ? void 0 : e.nodeType) === 11 && (e = t.parentNode), e;
  }
  get startNode() {
    return this._$AA;
  }
  get endNode() {
    return this._$AB;
  }
  _$AI(e, t = this) {
    e = R(this, e, t), N(e) ? e === c || e == null || e === "" ? (this._$AH !== c && this._$AR(), this._$AH = c) : e !== this._$AH && e !== P && this._(e) : e._$litType$ !== void 0 ? this.$(e) : e.nodeType !== void 0 ? this.T(e) : Te(e) ? this.k(e) : this._(e);
  }
  O(e) {
    return this._$AA.parentNode.insertBefore(e, this._$AB);
  }
  T(e) {
    this._$AH !== e && (this._$AR(), this._$AH = this.O(e));
  }
  _(e) {
    this._$AH !== c && N(this._$AH) ? this._$AA.nextSibling.data = e : this.T(E.createTextNode(e)), this._$AH = e;
  }
  $(e) {
    var a;
    const { values: t, _$litType$: r } = e, i = typeof r == "number" ? this._$AC(e) : (r.el === void 0 && (r.el = H.createElement(ye(r.h, r.h[0]), this.options)), r);
    if (((a = this._$AH) == null ? void 0 : a._$AD) === i) this._$AH.p(t);
    else {
      const o = new Ne(i, this), l = o.u(this.options);
      o.p(t), this.T(l), this._$AH = o;
    }
  }
  _$AC(e) {
    let t = pe.get(e.strings);
    return t === void 0 && pe.set(e.strings, t = new H(e)), t;
  }
  k(e) {
    X(this._$AH) || (this._$AH = [], this._$AR());
    const t = this._$AH;
    let r, i = 0;
    for (const a of e) i === t.length ? t.push(r = new D(this.O(U()), this.O(U()), this, this.options)) : r = t[i], r._$AI(a), i++;
    i < t.length && (this._$AR(r && r._$AB.nextSibling, i), t.length = i);
  }
  _$AR(e = this._$AA.nextSibling, t) {
    var r;
    for ((r = this._$AP) == null ? void 0 : r.call(this, !1, !0, t); e !== this._$AB; ) {
      const i = oe(e).nextSibling;
      oe(e).remove(), e = i;
    }
  }
  setConnected(e) {
    var t;
    this._$AM === void 0 && (this._$Cv = e, (t = this._$AP) == null || t.call(this, e));
  }
}
class q {
  get tagName() {
    return this.element.tagName;
  }
  get _$AU() {
    return this._$AM._$AU;
  }
  constructor(e, t, r, i, a) {
    this.type = 1, this._$AH = c, this._$AN = void 0, this.element = e, this.name = t, this._$AM = i, this.options = a, r.length > 2 || r[0] !== "" || r[1] !== "" ? (this._$AH = Array(r.length - 1).fill(new String()), this.strings = r) : this._$AH = c;
  }
  _$AI(e, t = this, r, i) {
    const a = this.strings;
    let o = !1;
    if (a === void 0) e = R(this, e, t, 0), o = !N(e) || e !== this._$AH && e !== P, o && (this._$AH = e);
    else {
      const l = e;
      let n, u;
      for (e = a[0], n = 0; n < a.length - 1; n++) u = R(this, l[r + n], t, n), u === P && (u = this._$AH[n]), o || (o = !N(u) || u !== this._$AH[n]), u === c ? e = c : e !== c && (e += (u ?? "") + a[n + 1]), this._$AH[n] = u;
    }
    o && !i && this.j(e);
  }
  j(e) {
    e === c ? this.element.removeAttribute(this.name) : this.element.setAttribute(this.name, e ?? "");
  }
}
class He extends q {
  constructor() {
    super(...arguments), this.type = 3;
  }
  j(e) {
    this.element[this.name] = e === c ? void 0 : e;
  }
}
class Me extends q {
  constructor() {
    super(...arguments), this.type = 4;
  }
  j(e) {
    this.element.toggleAttribute(this.name, !!e && e !== c);
  }
}
class De extends q {
  constructor(e, t, r, i, a) {
    super(e, t, r, i, a), this.type = 5;
  }
  _$AI(e, t = this) {
    if ((e = R(this, e, t, 0) ?? c) === P) return;
    const r = this._$AH, i = e === c && r !== c || e.capture !== r.capture || e.once !== r.once || e.passive !== r.passive, a = e !== c && (r === c || i);
    i && this.element.removeEventListener(this.name, this, r), a && this.element.addEventListener(this.name, this, e), this._$AH = e;
  }
  handleEvent(e) {
    var t;
    typeof this._$AH == "function" ? this._$AH.call(((t = this.options) == null ? void 0 : t.host) ?? this.element, e) : this._$AH.handleEvent(e);
  }
}
class ze {
  constructor(e, t, r) {
    this.element = e, this.type = 6, this._$AN = void 0, this._$AM = t, this.options = r;
  }
  get _$AU() {
    return this._$AM._$AU;
  }
  _$AI(e) {
    R(this, e);
  }
}
const J = k.litHtmlPolyfillSupport;
J == null || J(H, D), (k.litHtmlVersions ?? (k.litHtmlVersions = [])).push("3.3.2");
const je = (s, e, t) => {
  const r = (t == null ? void 0 : t.renderBefore) ?? e;
  let i = r._$litPart$;
  if (i === void 0) {
    const a = (t == null ? void 0 : t.renderBefore) ?? null;
    r._$litPart$ = i = new D(e.insertBefore(U(), a), a, void 0, t ?? {});
  }
  return i._$AI(s), i;
};
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const A = globalThis;
class w extends x {
  constructor() {
    super(...arguments), this.renderOptions = { host: this }, this._$Do = void 0;
  }
  createRenderRoot() {
    var t;
    const e = super.createRenderRoot();
    return (t = this.renderOptions).renderBefore ?? (t.renderBefore = e.firstChild), e;
  }
  update(e) {
    const t = this.render();
    this.hasUpdated || (this.renderOptions.isConnected = this.isConnected), super.update(e), this._$Do = je(t, this.renderRoot, this.renderOptions);
  }
  connectedCallback() {
    var e;
    super.connectedCallback(), (e = this._$Do) == null || e.setConnected(!0);
  }
  disconnectedCallback() {
    var e;
    super.disconnectedCallback(), (e = this._$Do) == null || e.setConnected(!1);
  }
  render() {
    return P;
  }
}
var de;
w._$litElement$ = !0, w.finalized = !0, (de = A.litElementHydrateSupport) == null || de.call(A, { LitElement: w });
const Z = A.litElementPolyfillSupport;
Z == null || Z({ LitElement: w });
(A.litElementVersions ?? (A.litElementVersions = [])).push("4.2.2");
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const ee = (s) => (e, t) => {
  t !== void 0 ? t.addInitializer(() => {
    customElements.define(s, e);
  }) : customElements.define(s, e);
};
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
const Ie = { attribute: !0, type: String, converter: L, reflect: !1, hasChanged: Q }, Le = (s = Ie, e, t) => {
  const { kind: r, metadata: i } = t;
  let a = globalThis.litPropertyMetadata.get(i);
  if (a === void 0 && globalThis.litPropertyMetadata.set(i, a = /* @__PURE__ */ new Map()), r === "setter" && ((s = Object.create(s)).wrapped = !0), a.set(t.name, s), r === "accessor") {
    const { name: o } = t;
    return { set(l) {
      const n = e.get.call(this);
      e.set.call(this, l), this.requestUpdate(o, n, s, !0, l);
    }, init(l) {
      return l !== void 0 && this.C(o, void 0, s, l), l;
    } };
  }
  if (r === "setter") {
    const { name: o } = t;
    return function(l) {
      const n = this[o];
      e.call(this, l), this.requestUpdate(o, n, s, !0, l);
    };
  }
  throw Error("Unsupported decorator location: " + r);
};
function me(s) {
  return (e, t) => typeof t == "object" ? Le(s, e, t) : ((r, i, a) => {
    const o = i.hasOwnProperty(a);
    return i.constructor.createProperty(a, r), o ? Object.getOwnPropertyDescriptor(i, a) : void 0;
  })(s, e, t);
}
/**
 * @license
 * Copyright 2017 Google LLC
 * SPDX-License-Identifier: BSD-3-Clause
 */
function b(s) {
  return me({ ...s, state: !0, attribute: !1 });
}
const v = "/umbraco/management/api/v1/umbcheckout";
async function z(s, e) {
  const t = await fetch(s, {
    ...e,
    headers: { "Content-Type": "application/json", ...e == null ? void 0 : e.headers }
  });
  if (!t.ok) throw new Error(`HTTP ${t.status}`);
  return t.json();
}
async function Be() {
  return z(`${v}/stripe-settings`);
}
async function Ke(s) {
  await fetch(`${v}/stripe-settings`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(s)
  });
}
async function qe() {
  return z(`${v}/stripe-shipping-rates/list`);
}
async function Ve() {
  return z(`${v}/stripe-shipping-rates/stripe`);
}
async function We(s) {
  return z(`${v}/stripe-shipping-rates/get?key=${s}`);
}
async function Je(s) {
  return z(`${v}/stripe-shipping-rates/stripe-detail?id=${s}`);
}
async function Ze(s) {
  await fetch(`${v}/stripe-shipping-rates/create`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(s)
  });
}
async function Fe(s, e) {
  await fetch(`${v}/stripe-shipping-rates/update?key=${e}`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(s)
  });
}
async function Ge(s) {
  await fetch(`${v}/stripe-shipping-rates/delete?key=${s}`, {
    method: "DELETE"
  });
}
var Ye = Object.defineProperty, Qe = Object.getOwnPropertyDescriptor, te = (s, e, t, r) => {
  for (var i = r > 1 ? void 0 : r ? Qe(e, t) : e, a = s.length - 1, o; a >= 0; a--)
    (o = s[a]) && (i = (r ? o(e, t, i) : o(i)) || i);
  return r && i && Ye(e, t, i), i;
};
let M = class extends w {
  constructor() {
    super(...arguments), this._properties = [], this._saveState = "init";
  }
  async connectedCallback() {
    super.connectedCallback();
    try {
      this._properties = await Be();
    } catch (s) {
      console.error(s);
    }
  }
  async _save() {
    this._saveState = "busy";
    try {
      const s = {};
      for (const e of this._properties) s[e.alias] = e.value;
      await Ke(s), this._saveState = "success", setTimeout(() => this._saveState = "init", 2e3);
    } catch {
      this._saveState = "error", setTimeout(() => this._saveState = "init", 2e3);
    }
  }
  _onChange(s, e) {
    this._properties = this._properties.map((t) => t.alias === s ? { ...t, value: e } : t);
  }
  render() {
    return d`
      <umb-body-layout header-transparent>
        <div slot="header"><h3>Stripe Settings</h3></div>
        <uui-box headline="Help &amp; Support">
          <p>Documentation available at <a href="https://docs.umbcheckout.net/" target="_blank">https://docs.umbcheckout.net/</a></p>
          <p>Please report issues on <a target="_blank" href="https://github.com/UmbHost/UmbCheckout/issues">GitHub</a></p>
        </uui-box>
        <uui-box headline="Configuration">
          ${this._properties.map((s) => d`
            <div class="property-row">
              <div class="property-label">${s.label}</div>
              <div class="property-description">${s.description}</div>
              ${s.view === "boolean" ? d`<uui-toggle .checked=${s.value === !0 || s.value === "true"} @change=${(e) => this._onChange(s.alias, e.target.checked)}></uui-toggle>` : d`<uui-input .value=${s.value ?? ""} @change=${(e) => this._onChange(s.alias, e.target.value)}></uui-input>`}
            </div>
          `)}
        </uui-box>
        <div slot="footer">
          <div class="footer">
            <uui-button look="primary" color="positive" label="Save" .state=${this._saveState === "busy" ? "waiting" : this._saveState === "success" ? "success" : this._saveState === "error" ? "failed" : void 0} @click=${this._save}>Save</uui-button>
          </div>
        </div>
      </umb-body-layout>
    `;
  }
};
M.styles = Y`
    :host { display: block; padding: var(--uui-size-layout-1); }
    uui-box { margin-bottom: var(--uui-size-layout-1); }
    .property-row { padding: var(--uui-size-space-4) 0; border-bottom: 1px solid var(--uui-color-border); }
    .property-label { font-weight: bold; margin-bottom: var(--uui-size-space-2); }
    .property-description { color: var(--uui-color-text-alt); font-size: 0.85em; margin-bottom: var(--uui-size-space-2); }
    .footer { display: flex; justify-content: flex-end; padding: var(--uui-size-space-4) 0; }
  `;
te([
  b()
], M.prototype, "_properties", 2);
te([
  b()
], M.prototype, "_saveState", 2);
M = te([
  ee("umbcheckout-stripe-settings")
], M);
var Xe = Object.defineProperty, et = Object.getOwnPropertyDescriptor, be = (s, e, t, r) => {
  for (var i = r > 1 ? void 0 : r ? et(e, t) : e, a = s.length - 1, o; a >= 0; a--)
    (o = s[a]) && (i = (r ? o(e, t, i) : o(i)) || i);
  return r && i && Xe(e, t, i), i;
};
let K = class extends w {
  constructor() {
    super(...arguments), this._rates = [];
  }
  async connectedCallback() {
    super.connectedCallback();
    try {
      this._rates = await qe();
    } catch (s) {
      console.error(s);
    }
  }
  _navigateToRate(s) {
    window.history.pushState({}, "", `/umbraco/section/settings/workspace/umbcheckout-stripe-shipping-rate/${s}`), window.dispatchEvent(new PopStateEvent("popstate"));
  }
  _navigateToCreate() {
    window.history.pushState({}, "", "/umbraco/section/settings/workspace/umbcheckout-stripe-shipping-rate/create"), window.dispatchEvent(new PopStateEvent("popstate"));
  }
  render() {
    return d`
      <umb-body-layout header-transparent>
        <div slot="header"><h3>Stripe Shipping Rates</h3></div>
        <uui-box headline="Help &amp; Support">
          <p>You will need to create your Shipping Rates within your Stripe account before you can add them below.</p>
        </uui-box>
        <div class="header-actions">
          <uui-button look="outline" label="Create Shipping Rate" @click=${this._navigateToCreate}>Create Shipping Rate</uui-button>
        </div>
        ${this._rates.length > 0 ? d`
            <uui-table>
              <uui-table-head>
                <uui-table-head-cell>Name</uui-table-head-cell>
                <uui-table-head-cell>Stripe ID</uui-table-head-cell>
              </uui-table-head>
              ${this._rates.map((s) => d`
                <uui-table-row @click=${() => this._navigateToRate(s.key)}>
                  <uui-table-cell>${s.name}</uui-table-cell>
                  <uui-table-cell>${s.value}</uui-table-cell>
                </uui-table-row>
              `)}
            </uui-table>` : d`<p>No shipping rates configured.</p>`}
      </umb-body-layout>
    `;
  }
};
K.styles = Y`
    :host { display: block; padding: var(--uui-size-layout-1); }
    uui-box { margin-bottom: var(--uui-size-layout-1); }
    uui-table { width: 100%; }
    uui-table-row { cursor: pointer; }
    uui-table-row:hover { background: var(--uui-color-surface-emphasis); }
    .header-actions { display: flex; justify-content: flex-end; padding: var(--uui-size-space-4) 0; }
  `;
be([
  b()
], K.prototype, "_rates", 2);
K = be([
  ee("umbcheckout-stripe-shipping-rates")
], K);
var tt = Object.defineProperty, it = Object.getOwnPropertyDescriptor, C = (s, e, t, r) => {
  for (var i = r > 1 ? void 0 : r ? it(e, t) : e, a = s.length - 1, o; a >= 0; a--)
    (o = s[a]) && (i = (r ? o(e, t, i) : o(i)) || i);
  return r && i && tt(e, t, i), i;
};
let $ = class extends w {
  constructor() {
    super(...arguments), this.rateKey = "", this._properties = [], this._stripeRates = [], this._selectedStripeRate = null, this._isEdit = !1, this._saveState = "init";
  }
  async connectedCallback() {
    super.connectedCallback();
    try {
      if (this._stripeRates = await Ve(), this.rateKey && this.rateKey !== "create") {
        this._isEdit = !0;
        const s = await We(this.rateKey);
        this._properties = s.properties;
        const e = this._properties.find((t) => t.alias === "value");
        e != null && e.value && (this._selectedStripeRate = await Je(e.value));
      } else
        this._properties = [
          { alias: "name", label: "Shipping Rate Name", description: "The Shipping Rate Name", value: "", view: "textbox", validation: { mandatory: !0 } },
          { alias: "value", label: "Shipping Rate ID", description: "The Shipping Rate ID set in Stripe", value: "", view: "textbox", validation: { mandatory: !0 } }
        ];
    } catch (s) {
      console.error(s);
    }
  }
  _onChange(s, e) {
    this._properties = this._properties.map((t) => t.alias === s ? { ...t, value: e } : t);
  }
  async _selectStripeRate(s) {
    this._selectedStripeRate = s, this._onChange("value", s.id), this._onChange("name", s.displayName);
  }
  async _save() {
    this._saveState = "busy";
    try {
      const s = {};
      for (const e of this._properties) s[e.alias] = e.value;
      this._isEdit ? await Fe(s, this.rateKey) : await Ze(s), this._saveState = "success", setTimeout(() => this._saveState = "init", 2e3);
    } catch {
      this._saveState = "error", setTimeout(() => this._saveState = "init", 2e3);
    }
  }
  async _delete() {
    if (confirm("Are you sure you want to delete this shipping rate?"))
      try {
        await Ge(this.rateKey), window.history.back();
      } catch (s) {
        console.error(s);
      }
  }
  render() {
    var s, e, t, r;
    return d`
      <umb-body-layout header-transparent>
        <div slot="header"><h3>${this._isEdit ? "Edit" : "Create"} Shipping Rate</h3></div>

        <uui-box headline="Configuration">
          ${this._properties.map((i) => d`
            <div class="property-row">
              <div class="property-label">${i.label}</div>
              <div class="property-description">${i.description}</div>
              <uui-input .value=${i.value ?? ""} @change=${(a) => this._onChange(i.alias, a.target.value)}></uui-input>
            </div>
          `)}
        </uui-box>

        ${this._selectedStripeRate ? d`
          <uui-box headline="Stripe Shipping Rate Details">
            <div class="detail-row"><span class="detail-label">Name</span><span>${this._selectedStripeRate.displayName}</span></div>
            <div class="detail-row"><span class="detail-label">Amount</span><span>${(s = this._selectedStripeRate.fixedAmount) == null ? void 0 : s.amount} ${(e = this._selectedStripeRate.fixedAmount) == null ? void 0 : e.currency}</span></div>
            ${(t = this._selectedStripeRate.deliveryEstimate) != null && t.minimum ? d`<div class="detail-row"><span class="detail-label">Minimum Delivery</span><span>${this._selectedStripeRate.deliveryEstimate.minimum.value} ${this._selectedStripeRate.deliveryEstimate.minimum.unit}</span></div>` : c}
            ${(r = this._selectedStripeRate.deliveryEstimate) != null && r.maximum ? d`<div class="detail-row"><span class="detail-label">Maximum Delivery</span><span>${this._selectedStripeRate.deliveryEstimate.maximum.value} ${this._selectedStripeRate.deliveryEstimate.maximum.unit}</span></div>` : c}
          </uui-box>
        ` : c}

        <uui-box headline="Available Stripe Shipping Rates">
          <p>Click on a Shipping Rate to update the form above</p>
          <uui-table>
            <uui-table-head>
              <uui-table-head-cell>Name</uui-table-head-cell>
              <uui-table-head-cell>ID</uui-table-head-cell>
            </uui-table-head>
            ${this._stripeRates.map((i) => d`
              <uui-table-row @click=${() => this._selectStripeRate(i)}>
                <uui-table-cell>${i.displayName}</uui-table-cell>
                <uui-table-cell>${i.id}</uui-table-cell>
              </uui-table-row>
            `)}
          </uui-table>
        </uui-box>

        <div slot="footer">
          <div class="footer">
            ${this._isEdit ? d`<uui-button look="primary" color="danger" label="Delete" @click=${this._delete}>Delete</uui-button>` : d`<div></div>`}
            <uui-button look="primary" color="positive" label="Save" .state=${this._saveState === "busy" ? "waiting" : this._saveState === "success" ? "success" : this._saveState === "error" ? "failed" : void 0} @click=${this._save}>Save</uui-button>
          </div>
        </div>
      </umb-body-layout>
    `;
  }
};
$.styles = Y`
    :host { display: block; padding: var(--uui-size-layout-1); }
    uui-box { margin-bottom: var(--uui-size-layout-1); }
    .property-row { padding: var(--uui-size-space-4) 0; border-bottom: 1px solid var(--uui-color-border); }
    .property-label { font-weight: bold; margin-bottom: var(--uui-size-space-2); }
    .property-description { color: var(--uui-color-text-alt); font-size: 0.85em; margin-bottom: var(--uui-size-space-2); }
    .detail-row { display: flex; justify-content: space-between; padding: var(--uui-size-space-2) 0; }
    .detail-label { font-weight: bold; }
    uui-table { width: 100%; }
    uui-table-row { cursor: pointer; }
    uui-table-row:hover { background: var(--uui-color-surface-emphasis); }
    .footer { display: flex; justify-content: space-between; padding: var(--uui-size-space-4) 0; }
  `;
C([
  me()
], $.prototype, "rateKey", 2);
C([
  b()
], $.prototype, "_properties", 2);
C([
  b()
], $.prototype, "_stripeRates", 2);
C([
  b()
], $.prototype, "_selectedStripeRate", 2);
C([
  b()
], $.prototype, "_isEdit", 2);
C([
  b()
], $.prototype, "_saveState", 2);
$ = C([
  ee("umbcheckout-stripe-shipping-rate")
], $);
//# sourceMappingURL=umbcheckout-stripe.js.map
